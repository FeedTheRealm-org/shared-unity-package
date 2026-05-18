using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.UI.AuthMenu
{
    public enum AuthPanel
    {
        Login,
        SignUp,
        VerifyNewAccount,
        ResetPassword,
        VerifyResetCode,
        NewPassword,
    }

    public class AuthFlowManager : MonoBehaviour
    {
        [Header("Shared Dependencies")]
        [SerializeField]
        private API.AuthService authService;

        [SerializeField]
        private Session.Session session;

        [SerializeField]
        private Logging.Logger logger;

        [Header("Panels")]
        [SerializeField]
        private GameObject loginPanel;

        [SerializeField]
        private GameObject signUpPanel;

        [SerializeField]
        private GameObject verifyNewAccountPanel;

        [SerializeField]
        private GameObject resetPasswordPanel;

        [SerializeField]
        private GameObject verifyResetCodePanel;

        [SerializeField]
        private GameObject newPasswordPanel;

        public event Action OnAuthComplete;
        public event Action OnAuthCancelled;
        private readonly Dictionary<AuthPanel, GameObject> states = new();

        private GameObject currentPanel;

        private void Awake()
        {
            DisableAllPanels();
            InitializeAuthFlow();
        }

        public void ShowAuthMenu()
        {
            ShowPanel(AuthPanel.Login);
        }

        public void ShowPanel(AuthPanel panel)
        {
            if (currentPanel != null)
                currentPanel.SetActive(false);
            if (states.TryGetValue(panel, out var panelObj))
            {
                panelObj.SetActive(true);
                currentPanel = panelObj;
            }
            else
            {
                logger.Log(
                    $"Panel {panel} not found in AuthFlowManager.",
                    this,
                    Logging.LogType.Error
                );
            }
        }

        public void CompleteAuth()
        {
            DisableAllPanels();
            OnAuthComplete?.Invoke();
        }

        public void CancelAuth()
        {
            DisableAllPanels();
            OnAuthCancelled?.Invoke();
        }

        private void InitializeAuthFlow()
        {
            InitializeAuthComponent(loginPanel.GetComponent<IAuth>());
            InitializeAuthComponent(signUpPanel.GetComponent<IAuth>());
            InitializeAuthComponent(verifyNewAccountPanel.GetComponent<IAuth>());
            states[AuthPanel.Login] = loginPanel;
            states[AuthPanel.SignUp] = signUpPanel;
            states[AuthPanel.VerifyNewAccount] = verifyNewAccountPanel;
        }

        private void DisableAllPanels()
        {
            loginPanel?.SetActive(false);
            signUpPanel?.SetActive(false);
            verifyNewAccountPanel?.SetActive(false);
            // resetPasswordPanel?.SetActive(false);
            // verifyResetCodePanel?.SetActive(false);
            // newPasswordPanel?.SetActive(false);
            currentPanel = null;
        }

        private void InitializeAuthComponent(IAuth authComponent)
        {
            if (authComponent == null)
            {
                logger.Log("IAuth component not found on panel.", this, Logging.LogType.Error);
                return;
            }
            authComponent.Initialize(authService, session, logger, this);
        }
    }
}
