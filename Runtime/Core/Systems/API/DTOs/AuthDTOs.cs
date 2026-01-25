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
}
