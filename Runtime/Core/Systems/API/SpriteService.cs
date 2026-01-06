using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "SpriteService", menuName = "Scriptable Objects/API/SpriteService")]
    public class SpriteService : ScriptableObject
    {
        [Header("Server settings")]
        [SerializeField]
        public string Hostname;

        [SerializeField]
        public int Port;

        [Header("General settings")]
        [SerializeField]
        private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/sprites";

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

            for (int i = 0; i < sprites.Count; i++)
            {
                (string spriteId, string spriteFilepath) = sprites[i];

                if (
                    spriteFilepath == null
                    || !File.Exists(spriteFilepath)
                    || string.IsNullOrEmpty(spriteId)
                )
                    continue;

                form.AddField($"ids[]", spriteId);
                byte[] spriteData = File.ReadAllBytes(spriteFilepath);
                form.AddBinaryData(
                    $"sprites[]",
                    spriteData,
                    Path.GetFileName(spriteFilepath),
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
