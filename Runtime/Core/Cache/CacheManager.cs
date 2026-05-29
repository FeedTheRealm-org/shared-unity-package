using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API;
using FTRShared.Runtime.Core.Interfaces;
using UnityEngine;

namespace FTRShared.Runtime.Core.Cache;

[Serializable]
public class CacheEntry
{
    public string uri;
    public string updatedAtIso;

    [NonSerialized]
    public DateTimeOffset updatedAt;

    public void SyncSerializedValues()
    {
        updatedAtIso = DateTimeHelper.ToIsoString(updatedAt);
    }

    public void HydrateRuntimeValues()
    {
        try
        {
            updatedAt = DateTimeHelper.ParseDateTimeOffset(updatedAtIso);
        }
        catch
        {
            updatedAt = DateTimeOffset.MinValue;
        }
    }
}

[Serializable]
public class CacheState
{
    public List<CacheEntry> entries = new List<CacheEntry>();
}

public class CacheManager : IDisposable
{
    private readonly AssetsService assetsService;
    private readonly ModelService modelService;
    private readonly IGltfLoader gltfLoaderService;
    private readonly DiskService disk;

    private readonly Dictionary<string, CacheEntry> cacheEntries =
        new Dictionary<string, CacheEntry>();
    private readonly Dictionary<string, int> cacheEntryIndex = new Dictionary<string, int>();

    private CacheState cacheState = new CacheState();
    private int pendingStateWrites = 0;
    private DateTime lastStateSaveUtc = DateTime.MinValue;
    private const int StateSaveWriteThreshold = 10;
    private static readonly TimeSpan StateSaveInterval = TimeSpan.FromSeconds(2);

    private const string cacheFolder = "cache/";
    private const string cacheStateFile = "cache/cache_state.json";

    public bool IsCachingEnabled { get; private set; } = true;

    public CacheManager(
        DiskService disk,
        AssetsService assetsService,
        IGltfLoader gltfLoaderService,
        ModelService modelService
    )
    {
        this.assetsService = assetsService;
        this.modelService = modelService;
        this.gltfLoaderService = gltfLoaderService;
        this.disk = disk;

        LoadCacheState();
    }

    public void Dispose()
    {
        SaveCacheState();
    }

    // Examples:
    // FULL URL: https://d3ry8oaxnx8r71.cloudfront.net/ArmorBody/f51a1c0e-07ad-4f3d-a647-82e61547aa4d.png
    // URI (unique and same for defaults): /ArmorBody/f51a1c0e-07ad-4f3d-a647-82e61547aa4d.png
    // URI(item - unique): /worlds/8ff4168b-137f-47f4-8887-f42fd3adc520/items/946a5ef4-c259-4dfa-a7b5-4493d07fa96f.png
    // URI(material - unique): /worlds/00000000-0000-0000-0000-000000000000/materials/c172b8c5-050f-4a83-bad7-0239ea48de25.jpg
    // BASE URL (remote): https://example.cloudfront.net
    // BASE URL (local): file://~/.config/unity3d/AtusGames/Feed the realm
    public async Task<Texture2D> GetSprite(string uri, DateTimeOffset updatedAt)
    {
        if (!IsCachingEnabled)
            return await assetsService.DownloadTexture2D(uri);

        var cachePath = Path.Combine(cacheFolder, uri.TrimStart('/'));
        Debug.Log($"Getting sprite for URI: {uri}, cache path: {cachePath}");
        byte[] data = disk.Read(cachePath);
        if (data == null || ShouldInvalidateCache(uri, updatedAt))
        {
            var newTexture = await assetsService.DownloadTexture2D(uri);
            if (newTexture != null)
            {
                disk.Write(cachePath, newTexture.EncodeToPNG());
                RegisterCacheEntry(new CacheEntry { uri = uri, updatedAt = updatedAt });
                TrySaveCacheState();
            }
            return newTexture;
        }

        Texture2D texture = DecodeTexture(data, uri);
        return texture;
    }

    // Examples:
    // URI (unique): /worlds/8ff4168b-137f-47f4-8887-f42fd3adc520/models/362d4df0-2ae8-4c15-9521-f2cadd69f8c3/user_uploaded_name.glb
    // URI (default models - unique): /worlds/00000000-0000-0000-0000-000000000000/models/7f141c6e-09f7-4c3d-ae16-1ea31f253888/DEFAULT_CHEST_CLOSED_chest_closed.glb
    // BASE URL (remote): https://example.cloudfront.net
    // BASE URL (local): file://~/.config/unity3d/AtusGames/Feed the realm
    public async Task<GameObject> GetModel(string uri, DateTimeOffset updatedAt)
    {
        if (!IsCachingEnabled)
        {
            var modelInfo = new ModelInfo { url = uri };
            var tempPath = await modelService.DownloadModel(
                modelInfo,
                savePath: null,
                isTemp: true
            );
            if (string.IsNullOrEmpty(tempPath))
                return null;

            byte[] tempData = File.ReadAllBytes(tempPath);
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            return await gltfLoaderService.LoadModel(tempData);
        }

        var cachePath = Path.Combine(cacheFolder, uri.TrimStart('/'));
        Debug.Log($"Getting model for URI: {uri}, cache path: {cachePath}");
        byte[] data = disk.Read(cachePath);
        if (data == null || ShouldInvalidateCache(uri, updatedAt))
        {
            var modelInfo = new ModelInfo { url = uri };
            var newModelPath = await modelService.DownloadModel(
                modelInfo,
                savePath: cachePath,
                isTemp: false
            );
            if (string.IsNullOrEmpty(newModelPath))
                return null;
            data = disk.Read(cachePath);
            if (data == null)
                return null;
            else
            {
                RegisterCacheEntry(new CacheEntry { uri = uri, updatedAt = updatedAt });
                TrySaveCacheState();
            }
        }

        GameObject model = await gltfLoaderService.LoadModel(data);
        return model;
    }

    private Texture2D DecodeTexture(byte[] data, string uri)
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, mipChain: false);

        if (!texture.LoadImage(data))
        {
            UnityEngine.Object.Destroy(texture);
            throw new System.Exception($"Failed to decode image data for URI: {uri}");
        }

        return texture;
    }

    private bool ShouldInvalidateCache(string uri, DateTimeOffset updatedAt)
    {
        if (cacheEntries.TryGetValue(uri, out var entry))
        {
            Debug.Log(
                $"updatedAt {updatedAt} > {entry.updatedAt} for URI: {uri}, should invalidate: {updatedAt > entry.updatedAt}."
            );
            return updatedAt > entry.updatedAt; // TODO: consider deleting file instead of just overwriting it
        }
        return true;
    }

    private void SaveCacheState()
    {
        foreach (var entry in cacheState.entries)
        {
            entry.SyncSerializedValues();
        }
        var cacheStateJson = JsonUtility.ToJson(cacheState);
        disk.Write(cacheStateFile, System.Text.Encoding.UTF8.GetBytes(cacheStateJson));
        pendingStateWrites = 0;
        lastStateSaveUtc = DateTime.UtcNow;
    }

    private void TrySaveCacheState()
    {
        pendingStateWrites++;
        if (
            pendingStateWrites >= StateSaveWriteThreshold
            || DateTime.UtcNow - lastStateSaveUtc >= StateSaveInterval
        )
            SaveCacheState();
    }

    private void RegisterCacheEntry(CacheEntry entry)
    {
        cacheEntries[entry.uri] = entry;
        if (!cacheEntryIndex.TryGetValue(entry.uri, out var index))
        {
            cacheEntryIndex[entry.uri] = cacheState.entries.Count;
            cacheState.entries.Add(entry);
            return;
        }

        if (index >= 0 && index < cacheState.entries.Count)
            cacheState.entries[index] = entry;
        else
            cacheEntryIndex.Remove(entry.uri);
    }

    private void LoadCacheState()
    {
        byte[] data = disk.Read(cacheStateFile);
        if (data == null)
            return;

        var cacheStateJson = System.Text.Encoding.UTF8.GetString(data);
        var loadedState = JsonUtility.FromJson<CacheState>(cacheStateJson);
        if (loadedState?.entries == null)
            return;

        cacheState = loadedState;
        cacheEntryIndex.Clear();
        for (int i = 0; i < loadedState.entries.Count; i++)
        {
            var entry = loadedState.entries[i];
            entry.HydrateRuntimeValues();
            cacheEntries[entry.uri] = entry;
            cacheEntryIndex[entry.uri] = i;
        }
    }

    public int ClearAllCache()
    {
        int deletedCount = 0;
        foreach (var entry in cacheEntries.Values)
        {
            var cachePath = Path.Combine(cacheFolder, entry.uri.TrimStart('/'));
            if (disk.Exists(cachePath))
            {
                disk.Delete(cachePath);
                deletedCount++;
            }
        }
        cacheEntries.Clear();
        cacheEntryIndex.Clear();
        cacheState.entries.Clear();
        if (disk.Exists(cacheStateFile))
        {
            disk.Delete(cacheStateFile);
            deletedCount++;
        }

        return deletedCount;
    }

    public void SetCachingEnabled(bool enabled)
    {
        IsCachingEnabled = enabled;
    }
}
