using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;

namespace API {
    [CreateAssetMenu(fileName = "ModelService", menuName = "Scriptable Objects/API/ModelService")]
    public class ModelService : ScriptableObject {
        [Header("Server settings")]
        [SerializeField] public string Hostname;
        [SerializeField] public int Port;

        [Header("General settings")]
        [SerializeField] private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/models";

        /// <summary>
        /// Downloads the .zip for the given worldId from /assets/models/{worldId}, saves it to a temp file,
        /// extracts it into destinationPath (creates directories as needed) and invokes callback with null on success
        /// or an error message on failure.
        /// </summary>
        public IEnumerator DownloadAndExtractAssets(string worldId, string accessToken, string destinationPath, System.Action<string> callback) {
            if (string.IsNullOrEmpty(worldId)) {
                callback?.Invoke("worldId is null or empty");
                yield break;
            }

            var url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";
            logger.Log($"Downloading assets zip from: {url}", this);

            var tempZipPath = Path.Combine(Application.temporaryCachePath, $"{worldId}.zip");

            // Ensure destination exists
            try {
                Directory.CreateDirectory(destinationPath);
            } catch (System.Exception ex) {
                logger.Log($"Failed to create destination directory: {ex.Message}", this, Logging.LogType.Error);
                callback?.Invoke(ex.Message);
                yield break;
            }

            var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerFile(tempZipPath);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            uwr.SetRequestHeader("Content-Type", "application/zip");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler == null ? uwr.error ?? string.Empty : $"Saved to {tempZipPath}";

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                logger.Log($"DownloadAssets error: {uwr.error}", this, Logging.LogType.Error);
                try { if (File.Exists(tempZipPath)) File.Delete(tempZipPath); } catch { }
                callback?.Invoke(uwr.error ?? "Download error");
                yield break;
            }


            try {
                if (!File.Exists(tempZipPath)) {
                    var msg = "Downloaded zip not found.";
                    logger.Log(msg, this, Logging.LogType.Error);
                    callback?.Invoke(msg);
                    yield break;
                }

                using (var archive = ZipFile.OpenRead(tempZipPath)) {
                    foreach (var entry in archive.Entries) {
                        var entryPath = Path.Combine(destinationPath, entry.FullName);
                        // If entry is a directory
                        if (string.IsNullOrEmpty(entry.Name)) {
                            Directory.CreateDirectory(entryPath);
                            continue;
                        }
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath) ?? destinationPath);
                        entry.ExtractToFile(entryPath, true);
                    }
                }

                logger.Log($"Assets extracted to: {destinationPath}", this);
                callback?.Invoke(null);
            } catch (System.Exception ex) {
                logger.Log($"Error extracting zip: {ex.Message}", this, Logging.LogType.Error);
                callback?.Invoke(ex.Message);
            } finally {
                // cleanup temp file
                try { if (File.Exists(tempZipPath)) File.Delete(tempZipPath); } catch { }
            }
        }

        /// <summary>
        /// Uploads asset model & material files for a world.
        /// </summary>
        public IEnumerator UploadAssets(List<Models.Asset> assets, string worldId, string accessToken, System.Action<string> callback) {
            if (assets == null || assets.Count == 0) {
                logger.Log("No assets to upload.", this, Logging.LogType.Warning);
                callback?.Invoke("No assets to upload.");
                yield break;
            }

            logger.Log($"Uploading {assets.Count} assets for world ID: {worldId}", this);

            var form = new WWWForm();

            // Add world_id
            form.AddField("world_id", worldId);

            // Add assets as multipart fields
            for (int i = 0; i < assets.Count; i++) {
                var asset = assets[i];
                string prefix = $"models[{i}]";

                form.AddField($"{prefix}.model_id", asset.Id);
                form.AddField($"{prefix}.name", asset.Name);

                byte[] modelData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.ModelPath));
                form.AddBinaryData(
                    $"{prefix}.model_file",
                    modelData,
                    Path.GetFileName(asset.ModelPath),
                    "application/octet-stream"
                );

                if (!string.IsNullOrEmpty(asset.MaterialPath)) {
                    form.AddField($"{prefix}.material_file", asset.MaterialPath);

                    byte[] materialData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.MaterialPath));
                    form.AddBinaryData(
                        $"{prefix}.material_file",
                        materialData,
                        Path.GetFileName(asset.MaterialPath),
                        "application/octet-stream"
                    );
                }
            }

            var url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success) {
                logger.Log("Assets uploaded successfully", this);
                callback?.Invoke(null);
            } else {
                logger.Log($"Asset upload error: {uwr.error}", this, Logging.LogType.Error);
                callback?.Invoke(uwr.error);
            }
        }


    }
}
