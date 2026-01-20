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
    }
}
