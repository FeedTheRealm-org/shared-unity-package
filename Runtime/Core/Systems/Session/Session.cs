using UnityEngine;

namespace Session {
    [CreateAssetMenu(fileName = "Session", menuName = "Scriptable Objects/Session")]
    public class Session : ScriptableObject {
        // Metadata
        public string APIToken { get; private set; }
        public bool IsFirstLogin { get; set; }

        // User Info
        public string Email { get; private set; }
        public string CharacterName { get; set; }

        public void SetAPIToken(string token) {
            APIToken = token;
        }

        public void SetEmail(string email) {
            Email = email;
        }

        public void ClearSession() {
            APIToken = null;
            Email = null;
            CharacterName = null;
        }
    }
}
