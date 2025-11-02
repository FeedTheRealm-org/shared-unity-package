using UnityEngine;
using System.Collections;

namespace API {
    /* --- Request --- */
    [System.Serializable]
    public class LoginRequest {
        public string email;
        public string password;
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
}
