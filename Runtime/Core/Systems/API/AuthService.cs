using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "AuthService", menuName = "Scriptable Objects/API/AuthService")]
    public class AuthService : BaseApiService
    {
        [SerializeField]
        private Session.Session session;

        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"http://{apiConfig.Hostname}:{apiConfig.Port}/auth";

        public IEnumerator Login(string email, string password, System.Action<string> handler)
        {
            var url = $"{GetBaseUrl()}/login";
            var payload = new LoginRequest { email = email, password = password };
            var json = JsonUtility.ToJson(payload);

            var task = SendRequestAsync(url, "POST", null, json, "Login");
            while (!task.IsCompleted)
                yield return null;
            var (responseText, result) = task.Result;

            try
            {
                if (
                    result == UnityWebRequest.Result.ConnectionError
                    || result == UnityWebRequest.Result.ProtocolError
                )
                {
                    var res = string.IsNullOrEmpty(responseText)
                        ? null
                        : JsonUtility.FromJson<ErrorResponse>(responseText);
                    logger.Log(
                        $"Login error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                        this,
                        Logging.LogType.Error
                    );
                    handler?.Invoke(
                        res != null && !string.IsNullOrEmpty(res.detail)
                            ? res.detail
                            : "Connection to the server failed."
                    );
                }
                else
                {
                    var res = JsonUtility.FromJson<DataEnvelope<LoginResponse>>(responseText);
                    logger.Log($"Login response: {responseText}", this);
                    logger.Log($"Login successful UserID: {res.data.id}", this);
                    session.SetUserId(res.data.id);
                    session.SetAPIToken(res.data.access_token);
                    session.SetEmail(res.data.email);
                    handler?.Invoke("");
                }
            }
            catch (System.Exception ex)
            {
                logger.Log($"Login exception: {ex.Message}", this, Logging.LogType.Error);
                handler?.Invoke("Connection to the server failed.");
            }
        }

        public IEnumerator SignUp(
            string email,
            string password,
            System.Action<bool, string> handler
        )
        {
            var url = $"{GetBaseUrl()}/signup";
            var payload = new LoginRequest { email = email, password = password };
            var json = JsonUtility.ToJson(payload);

            var task = SendRequestAsync(url, "POST", null, json, "SignUp");
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
                    $"SignUp error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(false, res?.detail ?? responseText);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<SignUpResponse>>(responseText);
                logger.Log($"SignUp response: {responseText}", this);
                logger.Log($"SignUp successful: {res.data.email}", this);
                handler?.Invoke(res.data.email == email, "");
            }
        }

        public IEnumerator VerifyCode(
            string email,
            string code,
            System.Action<bool, string> handler
        )
        {
            var url = $"{GetBaseUrl()}/verify";
            var payload = new VerifyCodeRequest { email = email, code = code };
            var json = JsonUtility.ToJson(payload);

            var task = SendRequestAsync(url, "POST", null, json, "VerifyCode");
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
                    $"Verify Code error: {(res != null ? $"{res.title}: {res.detail}" : responseText)} - {responseText}",
                    this,
                    Logging.LogType.Error
                );
                handler?.Invoke(false, res?.detail ?? responseText);
            }
            else
            {
                var res = JsonUtility.FromJson<DataEnvelope<VerifyCodeResponse>>(responseText);
                logger.Log($"Verify Code response: {responseText}", this);
                handler?.Invoke(res.data.verified, "");
            }
        }
    }
}
