using UnityEngine;
using UnityEngine.UIElements;

namespace FTRShared.UI.AuthMenu
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class AuthController : MonoBehaviour
    {
        protected API.AuthService authService;
        protected Session.Session session;
        protected Logging.Logger logger;
        protected AuthFlowManager flowManager;

        public void Initialize(
            API.AuthService authService,
            Session.Session session,
            Logging.Logger logger,
            AuthFlowManager flowManager
        )
        {
            this.authService = authService;
            this.session = session;
            this.logger = logger;
            this.flowManager = flowManager;
        }

        public void DisableCloseButton()
        {
            var ui = GetComponent<UIDocument>().rootVisualElement;
            var _closeButton = ui.Q<Button>("Close");
            if (_closeButton != null)
                _closeButton.style.display = DisplayStyle.None;
        }
    }
}
