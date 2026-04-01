using System.Collections;
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
        public IEnumerator GetWorldPage(
            int offset,
            int limit,
            string filter,
            string accessToken,
            System.Action<int, List<WorldData>, string> handler
        )
        {
            var url = $"{BaseUrl}?offset={offset}&limit={limit}";
            if (!string.IsNullOrWhiteSpace(filter))
                url += $"&filter={UnityWebRequest.EscapeURL(filter.Trim())}";

            var task = SendRequestAsync(url, "GET", accessToken, null, "GetWorldPage");
            while (!task.IsCompleted)
                yield return null;

            var (responseText, result, statusCode) = task.Result;
            var error = ParseError(result, responseText, statusCode, "GetWorldPage");
            if (error != null)
            {
                handler?.Invoke(0, null, error);
                yield break;
            }

            var envelope = JsonUtility.FromJson<DataEnvelope<WorldListResponse>>(responseText);
            if (envelope?.data == null)
            {
                handler?.Invoke(0, null, "Failed to parse world list.");
                yield break;
            }

            handler?.Invoke(envelope.data.amount, new List<WorldData>(), "");
        }

        /// <summary>
        /// Fetches a world and its creatables data by world id.
        /// Returns the deserialized WorldData and CreatablesData.
        /// </summary>
        public async Task<(WorldData worldData, CreatablesData creatablesData, string error, long statusCode)> GetWorld(
            string worldId, string accessToken)
        {
            var (responseText, result, statusCode) = await SendRequestAsync(
                $"{BaseUrl}/{worldId}", "GET", accessToken, null, "GetWorld");

            var error = ParseError(result, responseText, statusCode, "GetWorld");
            if (error != null) return (null, null, error, statusCode);

            var envelope = JsonUtility.FromJson<DataEnvelope<WorldDetailResponse>>(responseText);
            if (envelope?.data == null)
                return (null, null, "Failed to parse world response.", statusCode);

            WorldData worldData = JsonUtility.FromJson<WorldData>(envelope.data.data);
            if (worldData == null)
                return (null, null, "Failed to parse world data.", statusCode);

            worldData.worldId = envelope.data.id;
            worldData.worldName = envelope.data.name;

            CreatablesData creatablesData = JsonUtility.FromJson<CreatablesData>(envelope.data.createable_data);

            return (worldData, creatablesData, "", statusCode);
        }
    }
}
