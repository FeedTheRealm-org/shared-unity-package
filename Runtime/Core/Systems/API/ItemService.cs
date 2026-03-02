using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "ItemService", menuName = "Scriptable Objects/API/ItemService")]
    public class ItemService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"http://{apiConfig.Hostname}:{apiConfig.Port}/assets/items";

        /// <summary>
        /// Uploads sprite files for a world.
        /// The sprites are composed of tuples with (sprite_id, sprite_filepath)
        /// </summary>
        public async Task<string> UploadItemsByCategory(
            List<(string, string)> sprites,
            string worldId,
            string categoryId,
            string accessToken
        )
        {
            if (sprites == null || sprites.Count == 0)
            {
                logger.Log("No assets to upload.", this, Logging.LogType.Warning);
                return "No assets to upload.";
            }
            logger.Log($"Uploading {sprites.Count} assets for world ID: {worldId}", this);

            var form = new WWWForm();

            var validSprites = new List<(string spriteId, string absolutePath)>();

            for (int i = 0; i < sprites.Count; i++)
            {
                (string spriteId, string spriteFilePath) = sprites[i];

                if (spriteFilePath == null)
                {
                    continue;
                }

                string absolutePath = spriteFilePath;
                if (!Path.IsPathRooted(spriteFilePath))
                    absolutePath = Path.Combine(Application.streamingAssetsPath, spriteFilePath);

                if (!File.Exists(absolutePath))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(spriteId))
                {
                    logger.Log(
                        $"Sprite {i}: spriteId is null or empty",
                        this,
                        Logging.LogType.Warning
                    );
                    continue;
                }

                validSprites.Add((spriteId, absolutePath));
            }

            for (int j = 0; j < validSprites.Count; j++)
            {
                var (spriteId, absolutePath) = validSprites[j];

                form.AddField($"id[{j + 1}]", spriteId);
                byte[] spriteData = File.ReadAllBytes(absolutePath);
                logger.Log($"Adding sprite to form: {spriteId} (index {j + 1})", this);
                form.AddBinaryData(
                    $"sprite[{j + 1}]",
                    spriteData,
                    Path.GetFileName(absolutePath),
                    "application/octet-stream"
                );
            }
            var url = $"{GetBaseUrl()}/world/{worldId}/categories/{categoryId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.method = "PUT";
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                logger.Log("Assets uploaded successfully", this);
                return string.Empty;
            }
            else
            {
                logger.Log($"Asset upload error: {uwr.error}", this, Logging.LogType.Error);
                return uwr.error;
            }
        }

        /// <summary>
        /// Gets all item categories.
        /// </summary>
        public async Task<ItemCategoryListResponse> GetItemCategories(string accessToken)
        {
            string url = $"{GetBaseUrl()}/categories";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "GetItemCategories"
            );

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"GetItemCategories connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                throw new System.Exception(
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMessage = responseText;
                if (statusCode == 401)
                {
                    errorMessage = "Session expired. Please log in again.";
                }
                else if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"GetItemCategories error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                throw new System.Exception(errorMessage);
            }

            var response = JsonUtility.FromJson<DataEnvelope<ItemCategoryListResponse>>(
                responseText
            );
            return response.data;
        }
    }
}
