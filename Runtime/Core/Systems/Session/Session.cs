using System.Threading.Tasks;
using UnityEngine;

namespace Session
{
    [System.Serializable]
    public class SessionData
    {
        public string accessToken;
        public string refreshToken;
        public bool isFirstLogin;
        public string userID;
        public string email;
        public string characterName;
    }

    [CreateAssetMenu(fileName = "Session", menuName = "Scriptable Objects/Session")]
    public class Session : ScriptableObject
    {
        [SerializeField]
        private bool mustSaveSession = true;

        [SerializeField]
        private bool mustLoadSession = true;

        [SerializeField]
        private API.AuthService authService;

        [SerializeField]
        public string AccessToken = "";

        [SerializeField]
        public string RefreshToken = "";
        public bool IsFirstLogin { get; set; } = false;

        [SerializeField]
        public string UserID = "";

        [SerializeField]
        public string Email = "";

        public string Password { get; set; } = "";
        public string CharacterName { get; set; } = "";
        private SessionRepository _repository;

        private void OnEnable()
        {
            if (!mustLoadSession)
                return;

            _repository = new SessionRepository();
            var data = _repository.Load();

            if (data == null)
                return;

            AccessToken = data.accessToken;
            RefreshToken = data.refreshToken;
            IsFirstLogin = data.isFirstLogin;
            UserID = data.userID;
            Email = data.email;
            CharacterName = data.characterName;
        }

        private void OnDisable()
        {
            ClearMemory();
        }

        public void SetEmail(string email) => Email = email;

        public void SetPassword(string password) => Password = password;

        public void SetUserId(string id) => UserID = id;

        public void SaveSession()
        {
            if (!mustSaveSession)
                return;

            _repository.Save(
                new SessionData
                {
                    accessToken = AccessToken,
                    refreshToken = RefreshToken,
                    isFirstLogin = IsFirstLogin,
                    userID = UserID,
                    email = Email,
                    characterName = CharacterName,
                }
            );
        }

        public void ClearMemory()
        {
            AccessToken = "";
            RefreshToken = "";
            IsFirstLogin = false;
            UserID = "";
            Email = "";
            CharacterName = "";
        }

        public void ClearSession()
        {
            ClearMemory();
            _repository.Delete();
        }

        public async Task EnsureValidSession()
        {
            var (isLogged, _) = await authService.IsLogged();
            if (isLogged)
                return;

            var (refreshed, _) = await authService.RefreshToken(Email);
            if (!refreshed)
                return;
        }
    }
}
