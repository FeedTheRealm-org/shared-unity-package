using UnityEngine;

namespace Session {
    [CreateAssetMenu(fileName = "Session", menuName = "Scriptable Objects/Session")]
    public class Session : ScriptableObject {
        // Metadata
        [SerializeField] private string apiToken = "";
        [SerializeField] private bool isFirstLogin = false;

        public string APIToken { get => apiToken; private set => apiToken = value; }
        public bool IsFirstLogin { get => isFirstLogin; set => isFirstLogin = value; }

        // User Info
        public string UserId { get; set; } = "";
        public string Email { get; private set; } = "";
        public string CharacterName { get; set; } = "";

        public void SetAPIToken(string token) {
            Debug.Log($"Session: Setting API Token: {token}");
            APIToken = token;
        }

        public void SetEmail(string email) {
            Email = email;
        }

        public void ClearSession() {
            APIToken = "";
            IsFirstLogin = false;
            UserId = "";
            Email = "";
            CharacterName = "";
        }
    }
}
