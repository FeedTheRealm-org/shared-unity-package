using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates a new world on the server. Returns the server assigned world id.
        /// </summary>
        public async Task<(string id, string error, long statusCode)> PublishWorld(
            WorldData data,
            string accessToken
        )
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
                    accessToken,
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
                    $"World Cound not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return ("", ex.Message, 0);
            }
        }

        /// <summary>
        /// Updates an existing world on the server using its worldId.
        /// </summary>
        public async Task<(string id, string error, long statusCode)> UpdateWorld(
            WorldData data,
            string accessToken
        )
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
                accessToken,
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
            string worldId,
            string accessToken
        )
        {
            try
            {
                string json = JsonUtility.ToJson(new CreatablesRequest { createable_data = data });
                var (responseText, result, statusCode) = await SendRequestAsync(
                    $"{BaseUrl}/{worldId}/createable-data",
                    "PUT",
                    accessToken,
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
                    $"Creatables Cound not be Published: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                return (ex.Message, 0);
            }
        }

        /// <summary>
        /// Fetches a page of worlds from the server.
        /// </summary>
        /// <summary>
        /// Fetches a page of worlds from the server.
        /// </summary>
        public async Task<(int amount, List<WorldMetadata> worlds, string error)> GetWorldPage(
            int offset,
            int limit,
            string filter,
            string accessToken
        )
        {
            var url = $"{BaseUrl}?limit={limit}&offset={offset}";
            if (!string.IsNullOrWhiteSpace(filter))
                url += $"&filter={UnityWebRequest.EscapeURL(filter.Trim())}";

            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
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

        /// <summary>
        /// Fetches a world and its creatables data by world id.
        /// Returns the deserialized WorldData and CreatablesData.
        /// </summary>
        public async Task<(
            WorldData worldData,
            CreatablesData creatablesData,
            string error,
            long statusCode
        )> GetWorld(string worldId, string accessToken)
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{worldId}",
                "GET",
                accessToken,
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

        /// <summary>
        /// Fetches the server address (ip and port) for a specific zone in a world.
        /// </summary>
        public async Task<(string ip, int port, string error, long statusCode)> GetZoneAddress(
            string worldId,
            int zoneId,
            string accessToken
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/orchestrator/{worldId}/zones/{zoneId}/address",
                "GET",
                accessToken,
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

        /// <summary>
        /// Fetches a page of worlds and filters to only those with an active zone address.
        /// Returns world data paired with their active zone address.
        /// </summary>
        public async Task<(List<ActiveWorldData> activeWorlds, string error)> GetActiveWorlds(
            int offset,
            int limit,
            string filter,
            string accessToken
        )
        {
            var (amount, worlds, error) = await GetWorldPage(offset, limit, filter, accessToken);
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

            // fetch zones for each world then check for active address
            foreach (var world in worlds)
            {
                var (ip, port, addressError, _) = await GetZoneAddress(
                    world.id,
                    0, // Assume 0 or first zone for starting request
                    accessToken
                );
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

        public async Task<(string error, long statusCode)> DeleteWorld(
            string worldId,
            string accessToken
        )
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{worldId}",
                "DELETE",
                accessToken,
                null,
                "DeleteWorld"
            );

            var error = ParseError(result, responseText, statusCode, "DeleteWorld");
            return (error ?? "", statusCode);
        }
    }
}
