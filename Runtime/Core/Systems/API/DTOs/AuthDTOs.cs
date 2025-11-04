
namespace API {
    /* --- Request --- */
    [System.Serializable]
    public class LoginRequest {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class VerifyCodeRequest {
        public string email;
        public string code;
    }

    /* --- Responses --- */
    [System.Serializable]
    public class LoginResponse {
        public string message;
        public string token;
    }

    [System.Serializable]
    public class SignUpResponse {
        public string message;
        public string email;
    }

    [System.Serializable]
    public class VerifyCodeResponse {
        public string email;
        public bool verified;
    }
}
