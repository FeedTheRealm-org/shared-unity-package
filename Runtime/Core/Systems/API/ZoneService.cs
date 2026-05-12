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

        public async Task<(string id, string error, long statusCode)> PublishZone(
            string worldId,
            ZoneData data
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
                var (responseText, result, statusCode) = await SendRequestAsync(
                    $"{BaseUrl(worldId)}/{data.zoneId}",
                    "PUT",
                    session.AccessToken,
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
                    $"Zone Could not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return ("", ex.Message, 0);
            }
        }

        public async Task<(List<int> zones, string error, long statusCode)> GetZonesList(
            string worldId
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                BaseUrl(worldId),
                "GET",
                session.AccessToken,
                null,
                "GetZonesList"
            );

            var error = ParseError(result, responseText, statusCode, "GetZonesList");
            if (error != null)
                return (null, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<ZonesListResponse>>(responseText);
            return (envelope?.data?.zones ?? new List<int>(), "", statusCode);
        }

        public async Task<(ZoneData zoneData, string error, long statusCode)> GetZoneData(
            string worldId,
            int zoneId
        )
        {
            string url = $"{BaseUrl(worldId)}/{zoneId}";
            Debug.Log($"[ZoneService] Getting zone data from URL: {url}");
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                session.AccessToken,
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

        public async Task<(string error, long statusCode)> ActivateZone(string worldId, int zoneId)
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl(worldId)}/{zoneId}/activate",
                "GET",
                session.AccessToken,
                null,
                "ActivateZone"
            );

            var error = ParseError(result, responseText, statusCode, "ActivateZone");
            return (error, statusCode);
        }

        public async Task<(string error, long statusCode)> DeactivateZone(
            string worldId,
            int zoneId
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl(worldId)}/{zoneId}/deactivate",
                "GET",
                session.AccessToken,
                null,
                "DeactivateZone"
            );

            var error = ParseError(result, responseText, statusCode, "DeactivateZone");
            return (error, statusCode);
        }
    }
}
