using FTRShared.UI.AuthMenu;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LoginController : MonoBehaviour, IAuth
{
    private API.AuthService authService;
    private Session.Session session;
    private Logging.Logger logger;
    private AuthFlowManager flowManager;

    private VisualElement ui;
    private Button _loginButton;
    private Button _signUpChangeButton;
    private Button _recoverButton;
    private Button _closeButton;
    private TextField _emailField;
    private TextField _passwordField;
    private Label _messageError;

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

    private void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        _loginButton = ui.Q<Button>("LoginButton");
        _signUpChangeButton = ui.Q<Button>("SignUpChangeButton");
        _recoverButton = ui.Q<Button>("RecoverButton");
        _closeButton = ui.Q<Button>("Close");

        _loginButton.clicked += OnLoginClicked;
        _signUpChangeButton.clicked += OnNavigateToSignUp;
        _recoverButton.clicked += OnNavigateToRecover;
        _closeButton.clicked += OnClose;

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _messageError = ui.Q<Label>("MessageError");

        HideErrorMessage();
    }

    private void OnDisable()
    {
        if (_loginButton != null)
            _loginButton.clicked -= OnLoginClicked;
        if (_signUpChangeButton != null)
            _signUpChangeButton.clicked -= OnNavigateToSignUp;
        if (_recoverButton != null)
            _recoverButton.clicked -= OnNavigateToRecover;
        if (_closeButton != null)
            _closeButton.clicked -= OnClose;
    }

    private void OnNavigateToSignUp()
    {
        logger.Log("Navigating to Sign Up.", this);
        flowManager.ShowPanel(AuthPanel.SignUp);
    }

    private void OnNavigateToRecover()
    {
        logger.Log("Navigating to Reset Password.", this);
        flowManager.ShowPanel(AuthPanel.AccountRecovery);
    }

    private void OnClose()
    {
        logger.Log("Auth cancelled.", this);
        flowManager.CancelAuth();
    }

    private async void OnLoginClicked()
    {
        logger.Log("Login Button Clicked", this);
        HideErrorMessage();

        string err = await authService.Login(_emailField.value, _passwordField.value);

        if (!string.IsNullOrEmpty(err))
        {
            logger.Log("Login failed", this, Logging.LogType.Warning);

            if (err == "You must verify your email address before you can log in.")
            {
                session.SetEmail(_emailField.value);
                session.SetPassword(_passwordField.value);
                logger.Log("Navigating to Verify Code for verification.", this);
                flowManager.ShowPanel(AuthPanel.VerifyNewAccount);
            }

            ShowErrorMessage(err);
            return;
        }

        flowManager.AuthCompletion("Login successful.");
    }

    private void ShowErrorMessage(string err)
    {
        if (_messageError == null)
            return;

        _messageError.text = err switch
        {
            var e when e.Contains("verify your email address") =>
                "You must verify your email address.",
            var e when e.ToLower().Contains("connection") || e.ToLower().Contains("server") =>
                "Connection to the server failed.",
            var e
                when e.ToLower().Contains("credentials")
                    || e.ToLower().Contains("password")
                    || e.ToLower().Contains("email") => "Email or password is incorrect.",
            _ => err,
        };

        _messageError.style.display = DisplayStyle.Flex;
    }

    private void HideErrorMessage()
    {
        if (_messageError == null)
            return;
        _messageError.text = "";
        _messageError.style.display = DisplayStyle.None;
    }
}
