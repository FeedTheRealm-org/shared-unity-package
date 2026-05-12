using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(
        fileName = "BaseApiService",
        menuName = "Scriptable Objects/API/BaseApiService"
    )]
    public abstract class BaseApiService : ScriptableObject
    {
        [Header("Session settings")]
        [SerializeField]
        protected Session.Session session;

        [Header("General settings")]
        [SerializeField]
        protected Logging.Logger logger;

        /// <summary>
        /// Logic to send HTTP requests with authentication and logging.
        /// </summary>
        protected async Task<(
            string responseText,
            UnityWebRequest.Result result,
            long responseCode
        )> SendRequestAsync(
            string url,
            string method,
            string accessToken,
            string jsonBody = null,
            string logPrefix = null
        )
        {
            var response = await ExecuteRequestAsync(url, method, accessToken, jsonBody, logPrefix);

            if (response.responseCode == 401 && session != null)
            {
                logger.Log(
                    $"[{logPrefix}] 401 Unauthorized. Attempting to refresh session...",
                    this,
                    Logging.LogType.Warning
                );
                bool valid = await session.EnsureValidSession();
                if (valid)
                {
                    logger.Log(
                        $"[{logPrefix}] Session refreshed successfully. Retrying request...",
                        this
                    );
                    response = await ExecuteRequestAsync(
                        url,
                        method,
                        session.AccessToken,
                        jsonBody,
                        logPrefix
                    );
                }
            }

            return response;
        }

        private async Task<(
            string responseText,
            UnityWebRequest.Result result,
            long responseCode
        )> ExecuteRequestAsync(
            string url,
            string method,
            string accessToken,
            string jsonBody = null,
            string logPrefix = null
        )
        {
            UnityWebRequest uwr;
            if (method == "GET")
                uwr = UnityWebRequest.Get(url);
            else
                uwr = new UnityWebRequest(url, method);

            if (jsonBody != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            if (!string.IsNullOrEmpty(logPrefix))
                logger.Log($"{logPrefix} Request to {url}: {jsonBody}", this);

            await uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (!string.IsNullOrEmpty(logPrefix))
                logger.Log($"{logPrefix} response: {responseText}", this);

            return (responseText, uwr.result, uwr.responseCode);
        }

        protected string ParseError(
            UnityWebRequest.Result result,
            string responseText,
            long statusCode,
            string logPrefix
        )
        {
            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log($"[{logPrefix}] Connection error.", this, Logging.LogType.Error);
                return "Unable to connect to server. Please check your internet connection.";
            }

            if (result == UnityWebRequest.Result.ProtocolError)
            {
                string message = statusCode switch
                {
                    401 => "Session expired. Please log in again.",
                    >= 500 => "Server error. Please try again later.",
                    _ => JsonUtility.FromJson<ErrorResponse>(responseText)?.detail ?? responseText,
                };
                logger.Log(
                    $"[{logPrefix}] Error ({statusCode}): {message}",
                    this,
                    Logging.LogType.Error
                );
                return message;
            }

            return null;
        }
    }
}
