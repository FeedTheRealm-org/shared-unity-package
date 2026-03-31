using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTRShared.Runtime.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "ZoneService", menuName = "Scriptable Objects/API/ZoneService")]
    public class ZoneService : BaseApiService
    {
        [SerializeField]
        private ApiConfig apiConfig;

        private string BaseUrl(string worldId) =>
            $"{apiConfig.Hostname}:{apiConfig.Port}/world/{worldId}/zones";

        /// <summary>
        /// Publishes or Updates a zone on the server.
        /// </summary>
        public async Task<(string id, string error, long statusCode)> PublishZone(
            string worldId,
            ZoneData data,
            string accessToken
        )
        {
            try
            {
                string json = JsonUtility.ToJson(
                    new ZoneRequest
                    {
                        worldId = worldId,
                        zoneId = data.zoneId,
                        data = data,
                    }
                );
                string url = $"{BaseUrl(worldId)}/{data.zoneId}";
                var (responseText, result, statusCode) = await SendRequestAsync(
                    url,
                    "PUT",
                    accessToken,
                    json,
                    "PublishZone"
                );

                var error = ParseError(result, responseText, statusCode, "PublishZone");
                if (error != null)
                    throw new System.Exception(error);

                var res = JsonUtility.FromJson<DataEnvelope<ZoneResponse>>(responseText);
                return (res?.data?.world_id ?? "", "", statusCode);
            }
            catch (System.Exception ex)
            {
                logger.Log(
                    $"Zone Cound not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return ("", ex.Message, 0);
            }
        }
    }
}
