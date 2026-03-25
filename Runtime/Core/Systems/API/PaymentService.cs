using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(
        fileName = "PaymentService",
        menuName = "Scriptable Objects/API/PaymentService"
    )]
    public class PaymentService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetPaymentBaseUrl() =>
            $"http://{apiConfig.Hostname}:{apiConfig.Port}/payments";

        private string GetGemsBaseUrl() => $"{GetPaymentBaseUrl()}/gems";

        public async Task<(
            bool success,
            string message,
            List<GemPackResponse> packs
        )> GetAllGemPacks(string accessToken)
        {
            string url = $"{GetGemsBaseUrl()}/packs";

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "GET", accessToken, null, "GetAllGemPacks");

            try
            {
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    logger.Log(
                        $"GetAllGemPacks connection error: {responseText}",
                        this,
                        Logging.LogType.Error
                    );
                    return (
                        false,
                        "Unable to connect to server. Please check your internet connection.",
                        null
                    );
                }
                else if (result == UnityWebRequest.Result.ProtocolError)
                {
                    ErrorResponse res = string.IsNullOrEmpty(responseText)
                        ? null
                        : JsonUtility.FromJson<ErrorResponse>(responseText);
                    string errorMessage = res?.detail ?? responseText;
                    if (statusCode == 401 || statusCode == 403)
                    {
                        errorMessage = "Unauthorized access. Please log in again.";
                    }
                    else if (statusCode >= 500)
                    {
                        errorMessage = "Server error. Please try again later.";
                    }
                    logger.Log(
                        $"GetAllGemPacks error ({statusCode}): {errorMessage}",
                        this,
                        Logging.LogType.Error
                    );
                    return (false, errorMessage, null);
                }
                else
                {
                    DataEnvelope<List<GemPackResponse>> res = JsonUtility.FromJson<
                        DataEnvelope<List<GemPackResponse>>
                    >(responseText);
                    logger.Log($"CreateCheckoutSession response: {responseText}", this);
                    return (true, "", res.data);
                }
            }
            catch (System.Exception ex)
            {
                logger.Log($"GetAllGemPacks exception: {ex.Message}", this, Logging.LogType.Error);
                logger.Log(
                    $"GetAllGemPacks exception (result: {result}, status: {statusCode}): {ex.Message}",
                    this,
                    Logging.LogType.Error
                );

                string userMessage;
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    userMessage =
                        "Unable to connect to server. Please check your internet connection.";
                }
                else
                {
                    string statusInfo = statusCode > 0 ? $" (HTTP {statusCode})" : string.Empty;
                    userMessage =
                        $"Received an unexpected response from the server{statusInfo}. Please try again later.";
                }
                return (false, userMessage, null);
            }
        }

        public async Task<(bool success, string message, GemBalanceResponse balance)> GetGemBalance(
            string accessToken
        )
        {
            string url = $"{GetGemsBaseUrl()}/balances";

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "GET", accessToken, null, "GetGemBalance");

            try
            {
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    logger.Log(
                        $"GetGemBalance connection error: {responseText}",
                        this,
                        Logging.LogType.Error
                    );
                    return (
                        false,
                        "Unable to connect to server. Please check your internet connection.",
                        null
                    );
                }
                else if (result == UnityWebRequest.Result.ProtocolError)
                {
                    ErrorResponse res = string.IsNullOrEmpty(responseText)
                        ? null
                        : JsonUtility.FromJson<ErrorResponse>(responseText);
                    string errorMessage = res?.detail ?? responseText;
                    if (statusCode == 401)
                        errorMessage = "Unauthorized. Please log in again.";
                    else if (statusCode >= 500)
                        errorMessage = "Server error. Please try again later.";
                    logger.Log(
                        $"GetGemBalance error ({statusCode}): {errorMessage}",
                        this,
                        Logging.LogType.Error
                    );
                    return (false, errorMessage, null);
                }
                else
                {
                    DataEnvelope<GemBalanceResponse> res = JsonUtility.FromJson<
                        DataEnvelope<GemBalanceResponse>
                    >(responseText);
                    logger.Log($"GetGemBalance response: {responseText}", this);
                    return (true, "", res.data);
                }
            }
            catch (System.Exception ex)
            {
                logger.Log($"GetGemBalance exception: {ex.Message}", this, Logging.LogType.Error);
                string userMessage;
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    userMessage =
                        "Unable to connect to server. Please check your internet connection.";
                }
                else
                {
                    userMessage =
                        statusCode > 0
                            ? $"Unexpected response from server (status code {statusCode}). Please try again later."
                            : "Unexpected response from server. Please try again later.";
                }
                return (false, userMessage, null);
            }
        }

        public async Task<(
            bool success,
            string message,
            CheckoutResponse checkout
        )> CreateCheckoutSession(
            string gemPackId,
            string successUrl,
            string cancelUrl,
            string accessToken
        )
        {
            string url = $"{GetPaymentBaseUrl()}/checkout";
            CheckoutRequest payload = new CheckoutRequest
            {
                gem_pack_id = gemPackId,
                success_url = successUrl,
                cancel_url = cancelUrl,
            };
            string json = JsonUtility.ToJson(payload);

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "POST", accessToken, json, "CreateCheckoutSession");

            try
            {
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    logger.Log(
                        $"CreateCheckoutSession connection error: {responseText}",
                        this,
                        Logging.LogType.Error
                    );
                    return (
                        false,
                        "Unable to connect to server. Please check your internet connection.",
                        null
                    );
                }
                else if (result == UnityWebRequest.Result.ProtocolError)
                {
                    ErrorResponse res = string.IsNullOrEmpty(responseText)
                        ? null
                        : JsonUtility.FromJson<ErrorResponse>(responseText);
                    string errorMessage = res?.detail ?? responseText;
                    if (statusCode == 401)
                        errorMessage = "Unauthorized. Please log in again.";
                    else if (statusCode == 404)
                        errorMessage = "Gem pack not found.";
                    else if (statusCode >= 500)
                        errorMessage = "Server error. Please try again later.";
                    logger.Log(
                        $"CreateCheckoutSession error ({statusCode}): {errorMessage}",
                        this,
                        Logging.LogType.Error
                    );
                    return (false, errorMessage, null);
                }
                else
                {
                    DataEnvelope<CheckoutResponse> res = JsonUtility.FromJson<
                        DataEnvelope<CheckoutResponse>
                    >(responseText);
                    logger.Log($"CreateCheckoutSession response: {responseText}", this);
                    return (true, "", res.data);
                }
            }
            catch (System.Exception ex)
            {
                logger.Log(
                    $"CreateCheckoutSession exception: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                logger.Log(
                    $"CreateCheckoutSession exception (result: {result}, status: {statusCode}): {ex.Message}",
                    this,
                    Logging.LogType.Error
                );

                string userMessage;
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    userMessage =
                        "Unable to connect to server. Please check your internet connection.";
                }
                else
                {
                    string statusInfo = statusCode > 0 ? $" (HTTP {statusCode})" : string.Empty;
                    userMessage =
                        $"Received an unexpected response from the server{statusInfo}. Please try again later.";
                }
                return (false, userMessage, null);
            }
        }
    }
}
