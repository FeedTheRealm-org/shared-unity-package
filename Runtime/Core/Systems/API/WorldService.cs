using System.Collections.Generic;
using System.Threading.Tasks;
using FTRShared.Runtime.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "WorldService", menuName = "Scriptable Objects/API/WorldService")]
    public class WorldService : BaseApiService
    {
        [SerializeField]
        private ApiConfig apiConfig;

        private string BaseUrl => $"{apiConfig.Hostname}:{apiConfig.Port}/world";

        public async Task<(string id, string error, long statusCode)> PublishWorld(WorldData data)
        {
            try
            {
                string json = JsonUtility.ToJson(
                    new WorldRequest
                    {
                        file_name = data.worldName,
                        description = data.description,
                        data = data,
                    }
                );
                var (responseText, result, statusCode) = await SendRequestAsync(
                    BaseUrl,
                    "POST",
                    session.AccessToken,
                    json,
                    "CreateWorld"
                );

                var error = ParseError(result, responseText, statusCode, "CreateWorld");
                if (error != null)
                    return ("", error, statusCode);

                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                return (res?.data?.id ?? "", "", statusCode);
            }
            catch (System.Exception ex)
            {
                logger.Log(
                    $"World Could not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return ("", ex.Message, 0);
            }
        }

        public async Task<(string id, string error, long statusCode)> UpdateWorld(WorldData data)
        {
            string json = JsonUtility.ToJson(
                new WorldRequest
                {
                    file_name = data.worldName,
                    description = data.description,
                    data = data,
                }
            );
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{data.worldId}",
                "PUT",
                session.AccessToken,
                json,
                "UpdateWorld"
            );

            var error = ParseError(result, responseText, statusCode, "UpdateWorld");
            if (error != null)
                return ("", error, statusCode);

            return (data.worldId, "", statusCode);
        }

        public async Task<(string error, long statusCode)> PublishCreatables(
            CreatablesData data,
            string worldId
        )
        {
            try
            {
                string json = JsonUtility.ToJson(new CreatablesRequest { createable_data = data });
                var (responseText, result, statusCode) = await SendRequestAsync(
                    $"{BaseUrl}/{worldId}/createable-data",
                    "PUT",
                    session.AccessToken,
                    json,
                    "PublishCreatables"
                );

                var error = ParseError(result, responseText, statusCode, "PublishCreatables");
                if (error != null)
                    return (error, statusCode);
                return ("", statusCode);
            }
            catch (System.Exception ex)
            {
                logger.Log(
                    $"Creatables Could not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return (ex.Message, 0);
            }
        }

        public async Task<(int amount, List<WorldMetadata> worlds, string error)> GetWorldPage(
            int offset,
            int limit,
            string filter,
            bool getOwnWorlds = false
        )
        {
            var url = $"{BaseUrl}?limit={limit}&offset={offset}";
            if (!string.IsNullOrWhiteSpace(filter))
                url += $"&filter={UnityWebRequest.EscapeURL(filter.Trim())}";

            if (getOwnWorlds)
                url += $"&user_id={UnityWebRequest.EscapeURL(session.UserID)}";

            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                session.AccessToken,
                null,
                "GetWorldPage"
            );

            var error = ParseError(result, responseText, statusCode, "GetWorldPage");
            if (error != null)
                return (0, null, error);

            var envelope = JsonUtility.FromJson<DataEnvelope<WorldListResponse>>(responseText);
            if (envelope?.data == null)
                return (0, null, "Failed to parse world list.");

            return (envelope.data.amount, envelope.data.worlds, "");
        }

        public async Task<(
            WorldData worldData,
            CreatablesData creatablesData,
            string error,
            long statusCode
        )> GetWorld(string worldId)
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{worldId}",
                "GET",
                session.AccessToken,
                null,
                "GetWorld"
            );

            var error = ParseError(result, responseText, statusCode, "GetWorld");
            if (error != null)
                return (null, null, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<WorldDetailResponse>>(responseText);
            if (envelope?.data == null)
                return (null, null, "Failed to parse world response.", statusCode);

            WorldData worldData = JsonUtility.FromJson<WorldData>(envelope.data.data);
            if (worldData == null)
                return (null, null, "Failed to parse world data.", statusCode);

            worldData.worldId = envelope.data.id;
            worldData.worldName = envelope.data.name;

            CreatablesData creatablesData = JsonUtility.FromJson<CreatablesData>(
                envelope.data.createable_data
            );

            return (worldData, creatablesData, "", statusCode);
        }

        public async Task<(string ip, int port, string error, long statusCode)> GetZoneAddress(
            string worldId,
            int zoneId
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/orchestrator/{worldId}/zones/{zoneId}/address",
                "GET",
                session.AccessToken,
                null,
                "GetZoneAddress"
            );

            var error = ParseError(result, responseText, statusCode, "GetZoneAddress");
            if (error != null)
                return ("", 0, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<ZoneAddressResponse>>(responseText);
            if (envelope?.data == null)
                return ("", 0, "Failed to parse zone address.", statusCode);

            return (envelope.data.ip, envelope.data.port, "", statusCode);
        }

        public async Task<(List<ActiveWorldData> activeWorlds, string error)> GetActiveWorlds(
            int offset,
            int limit,
            string filter
        )
        {
            var (amount, worlds, error) = await GetWorldPage(offset, limit, filter);
            if (!string.IsNullOrEmpty(error))
            {
                logger.Log(
                    $"[Active Worlds] Failed to fetch world page: {error}",
                    this,
                    Logging.LogType.Error
                );
                return (null, error);
            }
            if (worlds == null || worlds.Count == 0)
                return (new List<ActiveWorldData>(), "");

            var activeWorlds = new List<ActiveWorldData>();
            logger.Log(
                $"[Active Worlds] Fetched {worlds.Count} worlds. Checking for active zones...",
                this,
                Logging.LogType.Info
            );

            foreach (var world in worlds)
            {
                var (ip, port, addressError, _) = await GetZoneAddress(world.id, 1);
                if (!string.IsNullOrEmpty(addressError))
                {
                    logger.Log(
                        $"[Active Worlds] Failed to fetch zone address for world {world.id}: {addressError}",
                        this,
                        Logging.LogType.Warning
                    );
                    continue;
                }
                activeWorlds.Add(
                    new ActiveWorldData
                    {
                        worldData = new WorldData
                        {
                            worldId = world.id,
                            worldName = world.name,
                            description = world.description ?? "",
                        },
                        zoneAddress = new ZoneAddressResponse { ip = ip, port = port },
                        updatedAt = world.updated_at,
                    }
                );
            }

            logger.Log(
                $"[Active Worlds] Found {activeWorlds.Count} active worlds.",
                this,
                Logging.LogType.Info
            );
            return (activeWorlds, "");
        }

        public async Task<(string error, long statusCode)> DeleteWorld(string worldId)
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{worldId}",
                "DELETE",
                session.AccessToken,
                null,
                "DeleteWorld"
            );

            var error = ParseError(result, responseText, statusCode, "DeleteWorld");
            return (error ?? "", statusCode);
        }
    }
}
