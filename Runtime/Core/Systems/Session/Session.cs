using UnityEngine;

namespace Session
{
    [CreateAssetMenu(fileName = "Session", menuName = "Scriptable Objects/Session")]
    public class Session : ScriptableObject
    {
        public string APIToken = "";

        public bool isFirstLogin = false;

        public bool IsFirstLogin
        {
            get => isFirstLogin;
            set => isFirstLogin = value;
        }

        private void OnEnable()
        {
            APIToken = "";
            UserId = "";
            Email = "";
            CharacterName = "";
        }

        // User Info
        public string UserId { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string Password { get; private set; } = "";
        public string CharacterName { get; set; } = "";

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
            APIToken = "";
            IsFirstLogin = false;
            UserId = "";
            Email = "";
            CharacterName = "";
        }
    }
}
