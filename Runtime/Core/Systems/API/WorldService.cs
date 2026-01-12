using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [System.Serializable]
    public class TempEnvelope
    {
        public InnerData data;
    }

    [System.Serializable]
    public class InnerData
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

        private string GetBaseUrl() => $"http://{apiConfig.Hostname}:{apiConfig.Port}/world";

        /// <summary>
        ///  Post a new world to the server or update an existing one if it has an id.
        /// </summary>
        public async Task<(string id, string error)> PublishWorld(
            Models.WorldData data,
            string fileName,
            string description,
            string accessToken
        )
        {
            if (!string.IsNullOrEmpty(data.id))
            {
                return await UpdateWorld(data, fileName, description, accessToken);
            }
            else
            {
                return await CreateWorld(data, fileName, description, accessToken);
            }
        }

        /// <summary>
        ///  Post a new world to the server (POST).
        /// </summary>
        private Task<(string id, string error)> CreateWorld(
            Models.WorldData data,
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
        private Task<(string id, string error)> UpdateWorld(
            Models.WorldData data,
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
        private async Task<(string id, string error)> SendWorldRequest(
            string url,
            string method,
            Models.WorldData data,
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

            var (responseText, result) = await SendRequestAsync(
                url,
                method,
                accessToken,
                json,
                logPrefix
            );

            if (
                result == UnityWebRequest.Result.ConnectionError
                || result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"{logPrefix} error: {res?.title}: {res?.detail}",
                    this,
                    Logging.LogType.Error
                );
                return ("", res?.detail ?? responseText);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                return (res?.data?.id ?? (method == "PUT" ? data.id : ""), "");
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
            System.Action<int, List<Models.WorldMetadata>, string> handler
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
            var (responseText, result) = task.Result;

            if (
                result == UnityWebRequest.Result.ConnectionError
                || result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetWorldPage error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(0, null, res?.detail ?? responseText);
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

                var worlds = new List<Models.WorldMetadata>();

                foreach (var worldItem in worldListResponse.worlds)
                {
                    try
                    {
                        var world = new Models.WorldMetadata();
                        world.id = worldItem.id;
                        world.userId = worldItem.user_id;
                        world.name = worldItem.name;
                        world.description = worldItem.description;
                        world.createdAt = worldItem.created_at;
                        world.updatedAt = worldItem.updated_at;

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
        /// Get world data using its ID from the server.
        /// </summary>
        public async Task<(Models.WorldData, string)> GetWorldData(
            string worldID,
            string accessToken
        )
        {
            var url = $"{GetBaseUrl()}/{worldID}";
            logger.Log($"Fetching world data from URL: {url}", this);
            var (responseText, result) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "GetWorldData"
            );

            if (
                result == UnityWebRequest.Result.ConnectionError
                || result == UnityWebRequest.Result.ProtocolError
            )
            {
                var res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetWorldData error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (null, res?.detail ?? responseText);
            }
            else
            {
                var tempEnvelope = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<TempEnvelope>(responseText);
                if (
                    tempEnvelope == null
                    || tempEnvelope.data == null
                    || string.IsNullOrEmpty(tempEnvelope.data.data)
                )
                {
                    return (null, "Failed to parse envelope");
                }

                var worldData = JsonUtility.FromJson<Models.WorldData>(tempEnvelope.data.data);
                if (worldData == null)
                {
                    return (null, "Failed to parse world data");
                }
                worldData.id = tempEnvelope.data.id;
                worldData.worldName = tempEnvelope.data.name ?? worldData.worldName;

                return (worldData, "");
            }
        }
    }
}
