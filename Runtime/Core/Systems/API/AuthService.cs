using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace API {

    [CreateAssetMenu(fileName = "AuthService", menuName = "Scriptable Objects/API/AuthService")]
    public class AuthService : ScriptableObject {
        [SerializeField]
        private Logging.Logger logger;

        [SerializeField]
        public string Hostname;

        [SerializeField]
        public int Port;

        [System.Serializable]
        class LoginPayload { public string email; public string password; }

        [System.Serializable]
        class VerifyCodePayload { public string email; public string code; }

        [System.Serializable]
        class LoginResponse {
            public Data data;
            [System.Serializable]
            public class Data { public string token; }
        }

        [System.Serializable]
        class SignUpResponse {
            public Data data;
            [System.Serializable]
            public class Data { public string email; }
        }


        [System.Serializable]
        class ErrorResponse {
            public string type;
            public string title;
            public int status;
            public string detail;
            public string instance;
        }

        public IEnumerator Login(string email, string password, System.Action<string, string, string> handler) {
            var url = $"http://{Hostname}:{Port}/auth/login";
            var payload = new LoginPayload { email = email, password = password };
            var json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"Login error: {(res != null ? res.title : responseText)} - {responseText}", this, Logging.LogType.Error);
                handler?.Invoke("", "", res.title);
            } else {
                var res = JsonUtility.FromJson<LoginResponse>(responseText);
                logger.Log($"Login response: {responseText}", this);
                logger.Log($"Login successful: {res.data.token}", this);
                handler?.Invoke(!string.IsNullOrEmpty(res.data.token) ? res.data.token : "", email, "");
            }
        }

        public IEnumerator SignUp(string email, string password, System.Action<bool, string> handler) {
            var url = $"http://{Hostname}:{Port}/auth/signup";
            var payload = new LoginPayload { email = email, password = password };
            var json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"SignUp error: {(res != null ? res.title : responseText)} - {responseText}", this, Logging.LogType.Error);
                handler?.Invoke(false, res.title);
            } else {
                var res = JsonUtility.FromJson<SignUpResponse>(responseText);
                logger.Log($"SignUp response: {responseText}", this);
                logger.Log($"SignUp successful: {res.data.email}", this);
                handler?.Invoke(res.data.email == email, "");
            }
        }

        public IEnumerator VerifyCode(string email, string code, System.Action<bool, string> handler) {
            var url = $"http://{Hostname}:{Port}/auth/verify";
            var payload = new VerifyCodePayload { email = email, code = code };
            var json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"VerifyCode error: {(res != null ? res.title : responseText)} - {responseText}", this, Logging.LogType.Error);
                handler?.Invoke(false, res.title);
            } else {
                logger.Log($"VerifyCode response: {responseText}", this);
                handler?.Invoke(true, "");
            }
        }
    }
}
