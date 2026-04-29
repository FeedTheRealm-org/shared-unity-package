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

        private string GetPaymentBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}/payments";

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
                        errorMessage = "Unauthorized access. Please log in again.";
                    else if (statusCode >= 500)
                        errorMessage = "Server error. Please try again later.";
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
                    logger.Log($"GetAllGemPacks response: {responseText}", this);
                    return (true, "", res.data);
                }
            }
            catch (System.Exception ex)
            {
                logger.Log($"GetAllGemPacks exception: {ex.Message}", this, Logging.LogType.Error);
                string userMessage = BuildUserMessage(result, statusCode);
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
                string userMessage = BuildUserMessage(result, statusCode);
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
                string userMessage = BuildUserMessage(result, statusCode);
                return (false, userMessage, null);
            }
        }

        public async Task<(
            bool success,
            string message,
            GemBalanceResponse updatedBalance
        )> PurchaseWithGems(string productId, string accessToken)
        {
            string url = $"{GetPaymentBaseUrl()}/purchase/{productId}";
            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "POST", accessToken, null, "PurchaseWithGems");

            try
            {
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    logger.Log(
                        $"PurchaseWithGems connection error: {responseText}",
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

                    if (statusCode == 400)
                        errorMessage = "You don't have enough gems.";
                    else if (statusCode == 404)
                        errorMessage = "Server error, cosmetic not found.";
                    else if (statusCode == 409)
                        errorMessage = "You already purchased this cosmetic.";
                    else if (statusCode == 401)
                        errorMessage = "Unauthorized. Please log in again.";
                    else if (statusCode >= 500)
                        errorMessage = "Server error. Please try again later.";

                    logger.Log(
                        $"PurchaseWithGems error ({statusCode}): {errorMessage}",
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
                    logger.Log($"PurchaseWithGems success: {responseText}", this);
                    return (true, "", res.data);
                }
            }
            catch (System.Exception ex)
            {
                logger.Log(
                    $"PurchaseWithGems exception: {ex.Message}",
                    this,
                    Logging.LogType.Error
                );
                string userMessage = BuildUserMessage(result, statusCode);
                return (false, userMessage, null);
            }
        }

        /// <summary>
        /// GET /payments/balances/creators
        /// The creator id is resolved server-side from the Authorization header.
        /// </summary>
        public async Task<(
            CreatorBalanceResponse data,
            string error,
            long statusCode
        )> GetCreatorBalance(string apiToken)
        {
            using var request = UnityWebRequest.Get($"{GetPaymentBaseUrl()}/balances/creators");
            request.SetRequestHeader("Authorization", $"Bearer {apiToken}");
            request.SetRequestHeader("Content-Type", "application/json");

            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            long status = request.responseCode;

            if (request.result != UnityWebRequest.Result.Success)
            {
                string body = request.downloadHandler?.text ?? string.Empty;
                return (null, string.IsNullOrEmpty(body) ? request.error : body, status);
            }

            try
            {
                var data = JsonUtility.FromJson<CreatorBalanceResponse>(
                    request.downloadHandler.text
                );
                return (data, null, status);
            }
            catch (System.Exception ex)
            {
                return (null, $"Parse error: {ex.Message}", status);
            }
        }

        private static string BuildUserMessage(UnityWebRequest.Result result, long statusCode)
        {
            if (result == UnityWebRequest.Result.ConnectionError)
                return "Unable to connect to server. Please check your internet connection.";

            string statusInfo = statusCode > 0 ? $" (HTTP {statusCode})" : string.Empty;
            return $"Received an unexpected response from the server{statusInfo}. Please try again later.";
        }
    }
}
