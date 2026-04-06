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
        [SerializeField]
        private ApiConfig apiConfig;

        private string BaseUrl => $"{apiConfig.Hostname}:{apiConfig.Port}/assets";

        /// <summary>
        /// Uploads sprite files for a world.
        /// Assumes all sprite paths have been validated before calling.
        /// </summary>
        public async Task<string> UploadSprites(
            SpritesRequest request,
            string worldId,
            string accessToken
        )
        {
            if (request.ids == null || request.ids.Count == 0)
                return "No assets to upload.";

            logger.Log(
                $"[SpriteService] Uploading {request.ids.Count} sprites for world {worldId}",
                this
            );

            var form = new WWWForm();
            for (int i = 0; i < request.ids.Count; i++)
            {
                form.AddField($"ids[{i}]", request.ids[i]);
                form.AddBinaryData(
                    $"sprites[{i}]",
                    File.ReadAllBytes(request.spritePath[i]),
                    Path.GetFileName(request.spritePath[i]),
                    "application/octet-stream"
                );
            }

            var uwr = UnityWebRequest.Put($"{BaseUrl}/items/world/{worldId}", form.data);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            uwr.SetRequestHeader(
                "Content-Type",
                $"multipart/form-data; boundary={form.headers["Content-Type"].Split('=')[1]}"
            );
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                logger.Log("[SpriteService] Sprites uploaded successfully.", this);
                return string.Empty;
            }

            logger.Log($"[SpriteService] Upload error: {uwr.error}", this, Logging.LogType.Error);
            return uwr.error;
        }
    }
}
