using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace API {
    [CreateAssetMenu(fileName = "WorldService", menuName = "Scriptable Objects/API/WorldService")]
    public class WorldService : ScriptableObject {
        [Header("Server settings")]
        [SerializeField] public string Hostname;
        [SerializeField] public int Port;

        [Header("General settings")]
        [SerializeField] private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/world";

        // ----------- CREATE WORLD (POST /world) -----------
        public IEnumerator CreateWorld(WorldRequest payload, string accessToken, System.Action<string, string> handler) {
            var url = GetBaseUrl();
            var json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"CreateWorld error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
                handler?.Invoke("", res?.detail ?? responseText);
            } else {
                logger.Log($"CreateWorld response: {responseText}", this);
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                handler?.Invoke(res?.data?.worldId, "");
            }
        }

        // ----------- GET WORLD (GET /world/{worldId}) -----------
        public IEnumerator GetWorldById(string worldId, string accessToken, System.Action<WorldData, string> handler) {
            var url = $"{GetBaseUrl()}/{worldId}";

            var uwr = UnityWebRequest.Get(url);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Accept", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"GetWorldById error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
                handler?.Invoke(null, res?.detail ?? responseText);
            } else {
                logger.Log($"GetWorldById response: {responseText}", this);
                var res = JsonUtility.FromJson<DataEnvelope<WorldData>>(responseText);
                handler?.Invoke(res?.data, "");
            }
        }
    }
}
