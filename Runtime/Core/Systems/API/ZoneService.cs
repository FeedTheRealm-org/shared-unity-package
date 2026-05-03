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

        /// <summary>
        /// Returns the list of zone ids for a given world.
        /// </summary>
        public async Task<(List<int> zones, string error, long statusCode)> GetZonesList(
            string worldId,
            string accessToken
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl(worldId)}",
                "GET",
                accessToken,
                null,
                "GetZonesList"
            );

            var error = ParseError(result, responseText, statusCode, "GetZonesList");
            if (error != null)
                return (null, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<ZonesListResponse>>(responseText);
            return (envelope?.data?.zones ?? new List<int>(), "", statusCode);
        }

        /// <summary>
        /// Returns the zone data for a specific zone in a world.
        /// </summary>
        public async Task<(ZoneData zoneData, string error, long statusCode)> GetZoneData(
            string worldId,
            int zoneId,
            string accessToken
        )
        {
            string url = $"{BaseUrl(worldId)}/{zoneId}";
            Debug.Log($"[ZoneService] Getting zone data from URL: {url}");
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "GetZoneData"
            );

            var error = ParseError(result, responseText, statusCode, "GetZoneData");
            if (error != null)
                return (null, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<ZoneResponse>>(responseText);
            if (envelope?.data == null)
                return (null, "Failed to parse zone response.", statusCode);

            var zoneData = JsonUtility.FromJson<ZoneData>(envelope.data.zone_data);
            return (zoneData, "", statusCode);
        }

        public async Task<(string error, long statusCode)> ActivateZone(
            string worldId,
            int zoneId,
            string accessToken
        )
        {
            string url = $"{BaseUrl(worldId)}/{zoneId}/activate";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "ActivateZone"
            );

            var error = ParseError(result, responseText, statusCode, "ActivateZone");
            return (error, statusCode);
        }

        public async Task<(string error, long statusCode)> DeactivateZone(
            string worldId,
            int zoneId,
            string accessToken
        )
        {
            string url = $"{BaseUrl(worldId)}/{zoneId}/deactivate";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "DeactivateZone"
            );

            var error = ParseError(result, responseText, statusCode, "DeactivateZone");
            return (error, statusCode);
        }
    }
}
