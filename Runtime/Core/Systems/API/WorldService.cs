using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FTRShared.Runtime.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [System.Serializable]
    public class WorldResponseEnvelope
    {
        public WorldResponseData data;
    }

    [System.Serializable]
    public class WorldResponseData
    {
        public string id;
        public string name;
        public string description;
        public string user_id;
        public string data;
    }

    [CreateAssetMenu(fileName = "WorldService", menuName = "Scriptable Objects/API/WorldService")]
    public class WorldService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}/world";

        /// <summary>
        ///  Post a new world to the server or update an existing one if it has an id.
        /// </summary>
        public async Task<(string id, string error, long statusCode)> PublishWorld(
            WorldData data,
            string fileName,
            string description,
            string accessToken
        )
        {
            if (!string.IsNullOrEmpty(data.id))
            {
                var (id, error, statusCode) = await UpdateWorld(
                    data,
                    fileName,
                    description,
                    accessToken
                );
                if (statusCode == 404)
                {
                    logger.Log(
                        $"PublishWorld: World '{data.id}' not found on server (404). Falling back to POST.",
                        this,
                        Logging.LogType.Warning
                    );
                    return await CreateWorld(data, fileName, description, accessToken);
                }
                return (id, error, statusCode);
            }
            else
            {
                return await CreateWorld(data, fileName, description, accessToken);
            }
        }

        /// <summary>
        ///  Post a new world to the server (POST).
        /// </summary>
        private Task<(string id, string error, long statusCode)> CreateWorld(
            FTRShared.Runtime.Models.WorldData data,
            string fileName,
            string description,
            string accessToken
        ) =>
            SendWorldRequest(
                GetBaseUrl(),
                "POST",
                data,
                fileName,
                description,
                accessToken,
                "CreateWorld"
            );

        /// <summary>
        ///  Update an existing world on the server (PUT).
        /// </summary>
        private Task<(string id, string error, long statusCode)> UpdateWorld(
            FTRShared.Runtime.Models.WorldData data,
            string fileName,
            string description,
            string accessToken
        ) =>
            SendWorldRequest(
                $"{GetBaseUrl()}/{data.id}",
                "PUT",
                data,
                fileName,
                description,
                accessToken,
                "UpdateWorld"
            );

        /// <summary>
        /// Send requests for creating or updating worlds.
        /// </summary>
        private async Task<(string id, string error, long statusCode)> SendWorldRequest(
            string url,
            string method,
            FTRShared.Runtime.Models.WorldData data,
            string fileName,
            string description,
            string accessToken,
            string logPrefix
        )
        {
            WorldRequest payload = new()
            {
                data = data,
                file_name = fileName,
                description = description,
            };

            string json = JsonUtility.ToJson(payload);

            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                method,
                accessToken,
                json,
                logPrefix
            );

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"{logPrefix} connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    "",
                    "Unable to connect to server. Please check your internet connection.",
                    0
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                var res = JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode == 401)
                {
                    errorMessage = "Session expired. Please log in again.";
                }
                else if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"{logPrefix} error ({statusCode}): {res?.title}: {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                return ("", errorMessage, statusCode);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                return (res?.data?.id ?? (method == "PUT" ? data.id : ""), "", statusCode);
            }
        }

        /// <summary>
        /// Get a page of worlds from the server.
        /// </summary>
        public IEnumerator GetWorldPage(
            int offset,
            int limit,
            string filter,
            string accessToken,
            System.Action<int, List<WorldMetadata>, string> handler
        )
        {
            var url = $"{GetBaseUrl()}?offset={offset}&limit={limit}";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var trimmed = filter.Trim();
                url = $"{url}&filter={UnityWebRequest.EscapeURL(trimmed)}";
            }
            logger.Log($"Fetching worlds from URL: {url}", this);
            var task = SendRequestAsync(url, "GET", accessToken, null, "GetWorldPage");
            while (!task.IsCompleted)
                yield return null;
            var (responseText, result, statusCode) = task.Result;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"GetWorldPage connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(
                    0,
                    null,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode == 401)
                {
                    errorMessage = "Session expired. Please log in again.";
                }
                else if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"GetWorldPage error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(0, null, errorMessage);
            }
            else
            {
                var envelope = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<DataEnvelope<WorldListResponse>>(responseText);
                var worldListResponse = envelope?.data;

                if (worldListResponse == null)
                {
                    handler?.Invoke(0, null, "Failed to parse world list response");
                    yield break;
                }

                var worlds = new List<WorldMetadata>();

                foreach (var worldItem in worldListResponse.worlds)
                {
                    try
                    {
                        var world = new WorldMetadata();
                        world.id = worldItem.id;
                        world.userId = worldItem.user_id;
                        world.name = worldItem.name;
                        world.description = worldItem.description;
                        world.createdAt = worldItem.created_at;
                        world.updatedAt = worldItem.updated_at;
                        var worldData = JsonUtility.FromJson<WorldData>(worldItem.data);
                        worlds.Add(world);
                    }
                    catch (System.Exception ex)
                    {
                        logger.Log(
                            $"Failed to parse world data for {worldItem.id}: {ex.Message}",
                            this,
                            Logging.LogType.Error
                        );
                    }
                }

                logger.Log($"GetWorldPage response: Loaded {worlds.Count} worlds", this);
                handler?.Invoke(worldListResponse.amount, worlds, "");
            }
        }

        /// <summary>
        /// Retrieves detailed world data from the server using the specified world ID.
        /// </summary>
        /// <param name="worldID">The unique identifier of the world to retrieve. This should be a valid world ID string as returned by world creation or listing endpoints. Typically a GUID or database-generated string.</param>
        /// <param name="accessToken">The access token for authenticating the request. Must be valid and authorized to access the specified world.</param>
        /// <returns>
        /// A tuple containing:
        ///   - <see cref="WorldData"/>: The deserialized world data object if retrieval and parsing succeed; otherwise, null.
        ///   - <see cref="string"/>: An error message if an error occurs, or an empty string on success.
        /// </returns>
        public async Task<(WorldData, string, long)> GetWorldData(
            string worldID,
            string accessToken
        )
        {
            var url = $"{GetBaseUrl()}/{worldID}";
            logger.Log($"Fetching world data from URL: {url}", this);
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "GetWorldData"
            );

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"GetWorldData connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    null,
                    "Unable to connect to server. Please check your internet connection.",
                    0
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode == 401)
                {
                    errorMessage = "Session expired. Please log in again.";
                }
                else if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"GetWorldData error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (null, errorMessage, statusCode);
            }
            else
            {
                var worldEnvelope = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<WorldResponseEnvelope>(responseText);
                if (
                    worldEnvelope == null
                    || worldEnvelope.data == null
                    || string.IsNullOrEmpty(worldEnvelope.data.data)
                )
                {
                    return (null, "Failed to parse envelope", statusCode);
                }

                var worldData = JsonUtility.FromJson<FTRShared.Runtime.Models.WorldData>(
                    worldEnvelope.data.data
                );
                if (worldData == null)
                {
                    return (null, "Failed to parse world data", statusCode);
                }
                worldData.id = worldEnvelope.data.id;
                worldData.worldName = worldEnvelope.data.name ?? worldData.worldName;

                return (worldData, "", statusCode);
            }
        }
    }
}
