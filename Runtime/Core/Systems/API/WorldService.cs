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
        public async Task<(string id, string error)> PublishWorld(
            Models.WorldData data,
            string fileName,
            string description,
            string accessToken
        )
        {
            logger.Log(
                $"WorldService.PublishWorld called with local id='{data?.id}', name='{data?.worldName}', file='{fileName}'",
                this
            );

            WorldRequest payload = new()
            {
                data = data,
                file_name = fileName,
                description = description,
            };

            string json = JsonUtility.ToJson(payload);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            bool hasExistingId = !string.IsNullOrEmpty(data?.id);

            logger.Log($"WorldService.PublishWorld hasExistingId={hasExistingId}", this);

            // If we already have an ID, try to update the existing world via PUT first.
            if (hasExistingId)
            {
                var putUrl = $"{GetBaseUrl()}/{data.id}";
                var putRequest = new UnityWebRequest(putUrl, "PUT");
                putRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                putRequest.downloadHandler = new DownloadHandlerBuffer();
                putRequest.SetRequestHeader("Content-Type", "application/json");
                putRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

                logger.Log($"Sending UpdateWorld (PUT) Request for id {data.id}: {json}", this);
                await putRequest.SendWebRequest();
                var putResponseText = putRequest.downloadHandler?.text
                    ?? putRequest.error
                    ?? string.Empty;

                logger.Log(
                    $"UpdateWorld PUT completed: result={putRequest.result}, code={putRequest.responseCode}, body='{putResponseText}'",
                    this
                );

                if (
                    putRequest.result == UnityWebRequest.Result.ConnectionError
                    || putRequest.result == UnityWebRequest.Result.ProtocolError
                )
                {
                    // If the world does not exist on the server, fall back to POST.
                    if (putRequest.responseCode == 404)
                    {
                        logger.Log(
                            $"UpdateWorld returned 404 for id {data.id}, falling back to create (POST). Response: {putResponseText}",
                            this,
                            Logging.LogType.Warning
                        );
                    }
                    else
                    {
                        var putError = JsonUtility.FromJson<ErrorResponse>(putResponseText);
                        logger.Log(
                            $"UpdateWorld error: {putError?.title}: {putError?.detail}",
                            this,
                            Logging.LogType.Error
                        );
                        return ("", putError?.detail ?? putResponseText);
                    }
                }
                else
                {
                    logger.Log($"UpdateWorld response: {putResponseText}", this);
                    return (data.id, "");
                }
            }

            // No existing id, or PUT failed with 404: create a new world via POST.
            var postUrl = GetBaseUrl();
            var postRequest = new UnityWebRequest(postUrl, "POST");
            postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            postRequest.downloadHandler = new DownloadHandlerBuffer();
            postRequest.SetRequestHeader("Content-Type", "application/json");
            postRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            logger.Log($"Sending CreateWorld (POST) Request: {json}", this);
            await postRequest.SendWebRequest();
            var postResponseText = postRequest.downloadHandler?.text
                ?? postRequest.error
                ?? string.Empty;

            logger.Log(
                $"CreateWorld POST completed: result={postRequest.result}, code={postRequest.responseCode}, body='{postResponseText}'",
                this
            );

            if (
                postRequest.result == UnityWebRequest.Result.ConnectionError
                || postRequest.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var postError = JsonUtility.FromJson<ErrorResponse>(postResponseText);
                logger.Log(
                    $"CreateWorld error: {postError?.title}: {postError?.detail}",
                    this,
                    Logging.LogType.Error
                );
                return ("", postError?.detail ?? postResponseText);
            }
            else
            {
                logger.Log($"CreateWorld response: {postResponseText}", this);
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(postResponseText);
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
                    var world = new Models.WorldMetadata();
                    world.id = worldItem.id;
                    world.userId = worldItem.user_id;
                    world.name = worldItem.name;
                    world.description = worldItem.description;
                    world.createdAt = worldItem.created_at;
                    world.updatedAt = worldItem.updated_at;

                    // The list endpoint now returns metadata only (no world data payload).
                    world.data = null;

                    worlds.Add(world);
                }

                logger.Log($"GetWorldPage response: Loaded {worlds.Count} worlds", this);
                handler?.Invoke(worldListResponse.amount, worlds, "");
            }
        }
    }
}
