using UnityEngine;
using System.Collections;

namespace API {
    [System.Serializable]
    class LoginPayload {
        public string email;
        public string password;
    }

    [System.Serializable]
    class LoginResponse {
        public string message;
        public string token;
    }

    [System.Serializable]
    class SignUpResponse {
        public string message;
        public string email;
    }
}
