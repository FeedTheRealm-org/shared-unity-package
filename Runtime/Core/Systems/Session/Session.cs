using UnityEngine;

namespace Session
{
    [CreateAssetMenu(fileName = "Session", menuName = "Scriptable Objects/Session")]
    public class Session : ScriptableObject
    {
        // Metadata
        [SerializeField]
        private string apiToken = "";

        [SerializeField]
        private bool isFirstLogin = false;

        public string APIToken
        {
            get => apiToken;
        }
        public bool IsFirstLogin
        {
            get => isFirstLogin;
            set => isFirstLogin = value;
        }

        // User Info
        public string UserId { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string Password { get; private set; } = "";
        public string CharacterName { get; set; } = "";

        public void SetAPIToken(string token)
        {
            apiToken = token;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetUserId(string id)
        {
            UserId = id;
        }

        public void ClearSession()
        {
            apiToken = "";
            IsFirstLogin = false;
            UserId = "";
            Email = "";
            CharacterName = "";
        }
    }
}
