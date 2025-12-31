using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "WorldService", menuName = "Scriptable Objects/API/WorldService")]
    public class WorldService : ScriptableObject
    {
        [Header("Server settings")]
        [SerializeField]
        public string Hostname;

        [SerializeField]
        public int Port;

        [Header("General settings")]
        [SerializeField]
        private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/world";

        /// <summary>
        ///  Post a new world to the server.
        /// </summary>
        public async Task<(string id, string error)> PublishWorld(Models.WorldData data, string fileName, string description, string accessToken)
        {
            logger.Log($"Uploading world data with these objects: {data.objectPlacementData}", this);

            WorldRequest payload = new()
            {
                data = data,
                file_name = fileName,
                description = description
            };

            string url = GetBaseUrl();
            string json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            logger.Log($"Sending Request: {json}", this);
            await uwr.SendWebRequest();
            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                var res = JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"CreateWorld error: {res?.title}: {res?.detail}", this, Logging.LogType.Error);
                return ("", res?.detail ?? responseText);
            }
            else
            {
                logger.Log($"CreateWorld response: {responseText}", this);
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                return (res?.data?.id ?? "", "");
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

            var uwr = UnityWebRequest.Get(url);
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            logger.Log($"Using API Token: {accessToken}", this);

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            logger.Log($"Worlds response text: {responseText}", this);

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
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

                        var worldData = JsonUtility.FromJson<Models.WorldData>(worldItem.data);
                        world.data = worldData;

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
    }
}
