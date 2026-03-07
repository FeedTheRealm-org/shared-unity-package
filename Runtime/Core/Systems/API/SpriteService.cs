using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FTRShared.Runtime.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "SpriteService", menuName = "Scriptable Objects/API/SpriteService")]
    public class SpriteService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() =>
            $"http://{apiConfig.Hostname}:{apiConfig.Port}/assets/sprites";

        /// <summary>
        /// Uploads sprite files for a world.
        /// The sprites are composed of tuples with (sprite_id, sprite_filepath)
        /// </summary>
        public async Task<string> UploadSprites(
            List<(string, string)> sprites,
            string worldId,
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
                    /*logger.Log(
                        $"Sprite {i}: spriteFilePath is null",
                        this,
                        Logging.LogType.Warning
                    );*/
                    continue;
                }

                string absolutePath = spriteFilePath;
                if (!Path.IsPathRooted(spriteFilePath))
                    absolutePath = Path.Combine(Application.streamingAssetsPath, spriteFilePath);

                if (!File.Exists(absolutePath))
                {
                    /*logger.Log(
                        $"Sprite {i}: file does not exist at {absolutePath}",
                        this,
                        Logging.LogType.Warning
                    );*/
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
                form.AddBinaryData(
                    $"sprite[{j + 1}]",
                    spriteData,
                    Path.GetFileName(absolutePath),
                    "application/octet-stream"
                );
            }
            var url = $"{GetBaseUrl()}/items/{worldId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
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
    }
}
