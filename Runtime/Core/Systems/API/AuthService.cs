using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "AuthService", menuName = "Scriptable Objects/API/AuthService")]
    public class AuthService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}/auth";

        public async Task<string> Login(string email, string password, string adminToken = null)
        {
            string url = $"{GetBaseUrl()}/login";
            LoginRequest payload = new LoginRequest { email = email, password = password };
            string json = JsonUtility.ToJson(payload);

            Task<(string, UnityWebRequest.Result, long)> task = SendRequestAsync(
                url,
                "POST",
                adminToken,
                json,
                "Login",
                false
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            try
            {
                if (result == UnityWebRequest.Result.ConnectionError)
                {
                    logger.Log(
                        $"Login connection error: {responseText}",
                        this,
                        Logging.LogType.Error
                    );
                    return "Unable to connect to server. Please check your internet connection.";
                }
                else if (result == UnityWebRequest.Result.ProtocolError)
                {
                    ErrorResponse res = string.IsNullOrEmpty(responseText)
                        ? null
                        : JsonUtility.FromJson<ErrorResponse>(responseText);
                    string errorMessage = res?.detail ?? responseText;
                    if (statusCode == 401)
                    {
                        errorMessage = "Invalid email or password.";
                    }
                    else if (statusCode >= 500)
                    {
                        errorMessage = "Server error. Please try again later.";
                    }
                    logger.Log(
                        $"Login error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                        this,
                        Logging.LogType.Error
                    );
                    return errorMessage;
                }
                else
                {
                    DataEnvelope<LoginResponse> res = JsonUtility.FromJson<
                        DataEnvelope<LoginResponse>
                    >(responseText);
                    logger.Log($"Login response: {responseText}", this);
                    logger.Log($"Login successful UserID: {res.data.id}", this);
                    session.ClearSession();
                    session.SetUserId(res.data.id);
                    session.AccessToken = res.data.access_token;
                    session.RefreshToken = res.data.refresh_token;
                    session.SetEmail(res.data.email);
                    session.SaveSession();
                    return "";
                }
            }
            catch (System.Exception ex)
            {
                logger.Log($"Login exception: {ex.Message}", this, Logging.LogType.Error);
                return "Connection to the server failed.";
            }
        }

        public async Task<(bool success, string message)> SignUp(string email, string password)
        {
            string url = $"{GetBaseUrl()}/signup";
            LoginRequest payload = new LoginRequest { email = email, password = password };
            string json = JsonUtility.ToJson(payload);

            Task<(string, UnityWebRequest.Result, long)> task = SendRequestAsync(
                url,
                "POST",
                null,
                json,
                "SignUp",
                false
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log($"SignUp connection error: {responseText}", this, Logging.LogType.Error);
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"SignUp error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<SignUpResponse> res = JsonUtility.FromJson<
                    DataEnvelope<SignUpResponse>
                >(responseText);
                logger.Log($"SignUp response: {responseText}", this);
                logger.Log($"SignUp successful: {res.data.email}", this);
                return (res.data.email == email, "");
            }
        }

        public async Task<(bool success, string message)> VerifyCode(string email, string code)
        {
            string url = $"{GetBaseUrl()}/verify";
            VerifyCodeRequest payload = new VerifyCodeRequest { email = email, code = code };
            string json = JsonUtility.ToJson(payload);

            Task<(string, UnityWebRequest.Result, long)> task = SendRequestAsync(
                url,
                "POST",
                null,
                json,
                "VerifyCode",
                false
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"Verify Code connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"Verify Code error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<VerifyCodeResponse> res = JsonUtility.FromJson<
                    DataEnvelope<VerifyCodeResponse>
                >(responseText);
                logger.Log($"Verify Code response: {responseText}", this);
                return (res.data.verified, "");
            }
        }

        public async Task<(bool success, string message)> RefreshVerification(string email)
        {
            string url = $"{GetBaseUrl()}/refresh";
            RefreshVerificationRequest payload = new RefreshVerificationRequest { email = email };
            string json = JsonUtility.ToJson(payload);

            Task<(string, UnityWebRequest.Result, long)> task = SendRequestAsync(
                url,
                "POST",
                null,
                json,
                "RefreshVerification",
                false
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"Refresh Verification connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"Refresh Verification error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<RefreshVerificationResponse> res = JsonUtility.FromJson<
                    DataEnvelope<RefreshVerificationResponse>
                >(responseText);
                logger.Log($"Refresh Verification response: {responseText}", this);
                return (true, "Your code has been refreshed.");
            }
        }

        public async Task<(bool success, string message)> RefreshToken(string email)
        {
            string url = $"{GetBaseUrl()}/refresh-token";
            RefreshTokenRequest payload = new RefreshTokenRequest { email = email };
            string json = JsonUtility.ToJson(payload);

            Task<(string, UnityWebRequest.Result, long)> task = ExecuteRequestAsync(
                url,
                "POST",
                session.RefreshToken,
                json,
                "RefreshToken"
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"Refresh Token connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"Refresh Token error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<RefreshTokenResponse> res = JsonUtility.FromJson<
                    DataEnvelope<RefreshTokenResponse>
                >(responseText);
                logger.Log($"Refresh Token response: {responseText}", this);
                session.AccessToken = res.data.access_token;
                session.RefreshToken = res.data.refresh_token;
                session.SaveSession();
                return (true, "");
            }
        }

        public async Task<(bool success, string message)> IsLogged()
        {
            string url = $"{GetBaseUrl()}/check-session";
            Task<(string, UnityWebRequest.Result, long)> task = ExecuteRequestAsync(
                url,
                "GET",
                session.AccessToken,
                "",
                "CheckSession"
            );
            (string responseText, UnityWebRequest.Result result, long statusCode) = await task;

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"Check Session connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"Check Session error ({statusCode}): {(res != null ? $"{res.title}: {errorMessage}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<CheckSessionResponse> res = JsonUtility.FromJson<
                    DataEnvelope<CheckSessionResponse>
                >(responseText);
                logger.Log($"Check Session response: {responseText}", this);
                return (true, res.data.message);
            }
        }

        public async Task<(bool success, string message)> ForgotPassword(string email)
        {
            string url = $"{GetBaseUrl()}/password/forgot";
            ForgotPasswordRequest payload = new ForgotPasswordRequest { email = email };
            string json = JsonUtility.ToJson(payload);

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "POST", null, json, "ForgotPassword", false);

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"ForgotPassword connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                    errorMessage = "Server error. Please try again later.";
                logger.Log(
                    $"ForgotPassword error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<ForgotPasswordResponse> res = JsonUtility.FromJson<
                    DataEnvelope<ForgotPasswordResponse>
                >(responseText);
                logger.Log($"ForgotPassword response: {responseText}", this);
                return (res.data.success, "");
            }
        }

        public async Task<(bool success, string resetToken, string message)> VerifyResetCode(
            string email,
            string code
        )
        {
            string url = $"{GetBaseUrl()}/password/verify-code";
            VerifyResetCodeRequest payload = new VerifyResetCodeRequest
            {
                email = email,
                code = code,
            };
            string json = JsonUtility.ToJson(payload);

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "POST", null, json, "VerifyResetCode", false);

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"VerifyResetCode connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    null,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                    errorMessage = "Server error. Please try again later.";
                logger.Log(
                    $"VerifyResetCode error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                return (false, null, errorMessage);
            }
            else
            {
                DataEnvelope<VerifyResetCodeResponse> res = JsonUtility.FromJson<
                    DataEnvelope<VerifyResetCodeResponse>
                >(responseText);
                logger.Log($"VerifyResetCode response: {responseText}", this);
                return (true, res.data.reset_token, "");
            }
        }

        public async Task<(bool success, string message)> ResetPassword(
            string resetToken,
            string newPassword
        )
        {
            string url = $"{GetBaseUrl()}/password/reset";
            ResetPasswordRequest payload = new ResetPasswordRequest
            {
                reset_token = resetToken,
                new_password = newPassword,
            };
            string json = JsonUtility.ToJson(payload);

            (string responseText, UnityWebRequest.Result result, long statusCode) =
                await SendRequestAsync(url, "POST", null, json, "ResetPassword", false);

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"ResetPassword connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return (
                    false,
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse res = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                string errorMessage = res?.detail ?? responseText;
                if (statusCode >= 500)
                    errorMessage = "Server error. Please try again later.";
                logger.Log(
                    $"ResetPassword error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                return (false, errorMessage);
            }
            else
            {
                DataEnvelope<ResetPasswordResponse> res = JsonUtility.FromJson<
                    DataEnvelope<ResetPasswordResponse>
                >(responseText);
                logger.Log($"ResetPassword response: {responseText}", this);
                return (res.data.success, "");
            }
        }
    }
}
