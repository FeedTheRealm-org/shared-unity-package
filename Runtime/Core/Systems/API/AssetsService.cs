using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "AssetsService", menuName = "Scriptable Objects/API/AssetsService")]
    public class AssetsService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}/assets/cosmetics";

        [System.Serializable]
        private class CosmeticResponse
        {
            public string cosmetic_id;
            public string cosmetic_url;
            public string updated_at;
        }

        [System.Serializable]
        private class CosmeticsListResponse
        {
            public CosmeticResponse[] cosmetics_list;
            public int total_count;
        }

        [System.Serializable]
        private class CategoryResponse
        {
            public string category_id;
            public string category_name;
        }

        private string GetCosmeticsCdnBaseUrl()
        {
            if (string.IsNullOrWhiteSpace(apiConfig.CosmeticsCDN))
                return string.Empty;
            return apiConfig.CosmeticsCDN.Trim().TrimEnd('/');
        }

        private string BuildSpriteDownloadUrl(string spriteUrl)
        {
            if (string.IsNullOrWhiteSpace(spriteUrl))
                return string.Empty;
            if (spriteUrl.StartsWith("http://") || spriteUrl.StartsWith("https://"))
                return spriteUrl;

            var path = spriteUrl.Trim();
            if (!path.StartsWith('/'))
                path = $"/{path}";

            var baseUrl = path.StartsWith("/worlds/")
                ? apiConfig.WorldsCDN.Trim().TrimEnd('/')
                : GetCosmeticsCdnBaseUrl();
            return string.IsNullOrEmpty(baseUrl) ? string.Empty : $"{baseUrl}{path}";
        }

        private static SpriteResponse MapToSpriteResponse(CosmeticResponse cosmetic)
        {
            if (cosmetic == null)
                return null;
            return new SpriteResponse
            {
                sprite_id = cosmetic.cosmetic_id,
                sprite_url = cosmetic.cosmetic_url,
                updated_at = cosmetic.updated_at,
            };
        }

        private static SpritesListResponse MapToSpritesListResponse(CosmeticsListResponse cosmetics)
        {
            var sprites = new SpritesListResponse
            {
                sprites_list = new SpriteResponse[cosmetics?.cosmetics_list?.Length ?? 0],
                total_count = cosmetics?.total_count ?? 0,
            };

            if (cosmetics?.cosmetics_list == null)
                return sprites;

            for (int i = 0; i < cosmetics.cosmetics_list.Length; i++)
                sprites.sprites_list[i] = MapToSpriteResponse(cosmetics.cosmetics_list[i]);

            return sprites;
        }

        public IEnumerator GetCategories(
            System.Action<SpriteCategoryListResponse, string> handler,
            bool isRetry = false
        )
        {
            var url = $"{GetBaseUrl()}/categories";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {base.session.AccessToken}");

            yield return uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                yield return session.EnsureValidSession();
                if (string.IsNullOrEmpty(session.AccessToken))
                    handler?.Invoke(null, "Unauthorized and failed to refresh session.");
                yield return GetCategories(handler, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetCategories error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(null, res.detail);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<SpriteCategoryListResponse>>(
                    responseText
                );
                handler?.Invoke(res.data, "");
            }
        }

        public IEnumerator GetSpritesByCategory(
            string categoryId,
            System.Action<SpritesListResponse, string> handler
        ) => GetSpritesByCategory(categoryId, 0, 24, handler);

        public IEnumerator GetSpritesByCategory(
            string categoryId,
            int offset,
            int limit,
            System.Action<SpritesListResponse, string> handler,
            bool isRetry = false
        )
        {
            var url =
                $"{GetBaseUrl()}/categories/{categoryId}?offset={Mathf.Max(0, offset)}&limit={Mathf.Max(1, limit)}";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            yield return uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                yield return session.EnsureValidSession();
                if (string.IsNullOrEmpty(session.AccessToken))
                    handler?.Invoke(null, "Unauthorized and failed to refresh session.");
                yield return GetSpritesByCategory(
                    categoryId,
                    offset,
                    limit,
                    handler,
                    isRetry: true
                );
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetSpritesByCategory error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(null, res.detail);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<CosmeticsListResponse>>(responseText);
                handler?.Invoke(MapToSpritesListResponse(res.data), "");
            }
        }

        public async Task<SpriteCategoryListResponse> GetCategoriesAsync(bool isRetry = false)
        {
            var url = $"{GetBaseUrl()}/categories";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");
            Debug.Log($"GetCategoriesAsync: Sending request to {url}");
            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await GetCategoriesAsync(isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetCategories error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }
            var envelope = JsonUtility.FromJson<DataEnvelope<SpriteCategoryListResponse>>(
                responseText
            );
            return envelope.data;
        }

        public async Task<SpritesListResponse> GetSpritesByCategoryAsync(
            string categoryId,
            int offset = 0,
            int limit = 24,
            string worldId = null,
            string playerId = null,
            bool isRetry = false
        )
        {
            if (string.IsNullOrWhiteSpace(worldId))
            {
                logger.Log(
                    "GetSpritesByCategoryAsync called without worldId.",
                    this,
                    Logging.LogType.Warning
                );
                worldId = new System.Guid().ToString();
            }

            var url =
                playerId == null
                    ? $"{GetBaseUrl()}/categories/{categoryId}?offset={Mathf.Max(0, offset)}&limit={Mathf.Max(1, limit)}&world_id={worldId}"
                    : $"{GetBaseUrl()}/categories/{categoryId}?offset={Mathf.Max(0, offset)}&limit={Mathf.Max(1, limit)}&world_id={worldId}&player_id={playerId}";

            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await GetSpritesByCategoryAsync(
                    categoryId,
                    offset,
                    limit,
                    worldId,
                    playerId,
                    isRetry: true
                );
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetSpritesByCategory error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }
            var envelope = JsonUtility.FromJson<DataEnvelope<CosmeticsListResponse>>(responseText);
            return MapToSpritesListResponse(envelope.data);
        }

        public async Task<SpriteResponse> GetSpriteByIdAsync(string spriteId, bool isRetry = false)
        {
            var url = $"{GetBaseUrl()}/{spriteId}";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await GetSpriteByIdAsync(spriteId, isRetry: true);
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetSpriteById error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }
            var envelope = JsonUtility.FromJson<DataEnvelope<CosmeticResponse>>(responseText);
            return MapToSpriteResponse(envelope.data);
        }

        public async Task<SpriteResponse> UploadSpriteAsync(
            string categoryId,
            string spritePath,
            string worldId,
            int price = 1,
            bool isRetry = false
        )
        {
            var url = $"{GetBaseUrl()}/categories/{categoryId}";
            var formData = new List<IMultipartFormSection>();
            byte[] fileData = System.IO.File.ReadAllBytes(spritePath);
            formData.Add(
                new MultipartFormFileSection(
                    "sprite",
                    fileData,
                    System.IO.Path.GetFileName(spritePath),
                    "image/png"
                )
            );
            formData.Add(new MultipartFormDataSection("category_id", categoryId));
            formData.Add(new MultipartFormDataSection("world_id", worldId ?? string.Empty));
            formData.Add(new MultipartFormDataSection("price", price.ToString()));

            var uwr = UnityWebRequest.Post(url, formData);
            uwr.method = "PUT";
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return null;
                return await UploadSpriteAsync(
                    categoryId,
                    spritePath,
                    worldId,
                    price,
                    isRetry: true
                );
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                logger.Log(
                    $"UploadSpriteAsync error: {responseText}",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }
            var res = JsonUtility.FromJson<DataEnvelope<CosmeticResponse>>(responseText);
            return MapToSpriteResponse(res.data);
        }

        public async Task<(SpriteResponse response, long statusCode)> LinkSpriteByIdAsync(
            string categoryId,
            string spriteId,
            string worldId,
            int price = 1,
            string spritePath = null,
            bool isRetry = false
        )
        {
            var url = $"{GetBaseUrl()}/categories/{categoryId}/sprites/{spriteId}";
            var formData = new List<IMultipartFormSection>();

            if (!string.IsNullOrEmpty(spritePath) && System.IO.File.Exists(spritePath))
            {
                byte[] fileData = System.IO.File.ReadAllBytes(spritePath);
                formData.Add(
                    new MultipartFormFileSection(
                        "sprite",
                        fileData,
                        System.IO.Path.GetFileName(spritePath),
                        "image/png"
                    )
                );
            }
            else
            {
                // Fallback dummy file to force multipart/form-data
                formData.Add(
                    new MultipartFormFileSection(
                        "dummy_force_multipart",
                        new byte[0],
                        "dummy.bin",
                        "application/octet-stream"
                    )
                );
            }
            formData.Add(new MultipartFormDataSection("world_id", worldId ?? string.Empty));
            formData.Add(new MultipartFormDataSection("price", price.ToString()));

            var uwr = UnityWebRequest.Post(url, formData);
            uwr.method = "PUT";
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");

            await uwr.SendWebRequest();

            if (uwr.responseCode == 401 && !isRetry)
            {
                var result = await session.EnsureValidSession();
                if (!result)
                    return (null, uwr.responseCode);
                return await LinkSpriteByIdAsync(
                    categoryId,
                    spriteId,
                    worldId,
                    price,
                    spritePath,
                    isRetry: true
                );
            }

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                logger.Log(
                    $"LinkSpriteByIdAsync error (status {uwr.responseCode}): {responseText}",
                    this,
                    Logging.LogType.Warning
                );
                return (null, uwr.responseCode);
            }
            var res = JsonUtility.FromJson<DataEnvelope<CosmeticResponse>>(responseText);
            return (MapToSpriteResponse(res.data), uwr.responseCode);
        }

        public async Task<Texture2D> DownloadTexture2D(string spriteReference)
        {
            if (string.IsNullOrWhiteSpace(spriteReference))
            {
                logger.Log(
                    "DownloadTexture2D called with empty sprite reference",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }

            string spriteUrl = spriteReference;
            if (System.Guid.TryParse(spriteReference, out _))
            {
                var sprite = await GetSpriteByIdAsync(spriteReference);
                if (sprite == null || string.IsNullOrWhiteSpace(sprite.sprite_url))
                {
                    logger.Log(
                        $"DownloadTexture2D could not resolve sprite URL for id: {spriteReference}",
                        this,
                        Logging.LogType.Error
                    );
                    return null;
                }
                spriteUrl = sprite.sprite_url;
            }

            var url = BuildSpriteDownloadUrl(spriteUrl);
            if (string.IsNullOrEmpty(url))
            {
                logger.Log(
                    $"DownloadTexture2D invalid CosmeticsCDN or sprite URL. CosmeticsCDN: '{apiConfig.CosmeticsCDN}', sprite_url: '{spriteUrl}'",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            using var uwr = UnityWebRequestTexture.GetTexture(url);
            await uwr.SendWebRequest();

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"DownloadTexture2D error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var texture = DownloadHandlerTexture.GetContent(uwr);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            logger.Log($"DownloadTexture2D success for sprite_url: {spriteUrl}", this);
            return texture;
        }
    }
}
