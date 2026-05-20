namespace API
{
    /* --- Request --- */
    [System.Serializable]
    public class LoginRequest
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class VerifyCodeRequest
    {
        public string email;
        public string code;
    }

    /* --- Responses --- */
    [System.Serializable]
    public class LoginResponse
    {
        public string access_token;
        public string refresh_token;
        public string id;
        public string email;
        public string createdAt;
        public string updatedAt;
    }

    [System.Serializable]
    public class SignUpResponse
    {
        public string email;
    }

    [System.Serializable]
    public class VerifyCodeResponse
    {
        public string email;
        public bool verified;
    }

    [System.Serializable]
    public class RefreshVerificationRequest
    {
        public string email;
    }

    [System.Serializable]
    public class RefreshVerificationResponse
    {
        public string email;
    }

    [System.Serializable]
    public class RefreshTokenRequest
    {
        public string email;
    }

    [System.Serializable]
    public class RefreshTokenResponse
    {
        public string access_token;
        public string refresh_token;
    }

    [System.Serializable]
    public class CheckSessionResponse
    {
        public string message;
    }

    // --- Password Reset Requests ---
    [System.Serializable]
    public class ForgotPasswordRequest
    {
        public string email;
    }

    [System.Serializable]
    public class VerifyResetCodeRequest
    {
        public string email;
        public string code;
    }

    [System.Serializable]
    public class ResetPasswordRequest
    {
        public string reset_token;
        public string new_password;
    }

    // --- Password Reset Responses ---
    [System.Serializable]
    public class ForgotPasswordResponse
    {
        public bool success;
    }

    [System.Serializable]
    public class VerifyResetCodeResponse
    {
        public string reset_token;
    }

    [System.Serializable]
    public class ResetPasswordResponse
    {
        public bool success;
    }
}
