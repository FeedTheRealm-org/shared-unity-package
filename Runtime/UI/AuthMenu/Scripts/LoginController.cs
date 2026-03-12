using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [SerializeField]
    private SceneReference targetScene;

    [SerializeField]
    private SceneReference otherFormScene;

    [SerializeField]
    private SceneReference verifyCodeScene;

    [SerializeField]
    private Logging.Logger logger;

    public event Action OnNavigateToSignUp;
    public event Action OnNavigateToVerifyCode;
    public event Action OnLoginSuccess;

    private VisualElement ui;

    private Button _loginButton;
    private TextField _emailField;
    private TextField _passwordField;
    private Label _changeButton;
    private Label _messageError;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        logger.Log("LoginController enabled.", this);

        _loginButton = ui.Q<Button>("LoginButton");
        _loginButton.clicked += OnLoginClicked;

        _changeButton = ui.Q<Label>("SignUpChangeButton");
        _changeButton.RegisterCallback<ClickEvent>(evt =>
        {
            if (OnNavigateToSignUp != null)
            {
                logger.Log("Navigating to Sign Up.", this);
                OnNavigateToSignUp.Invoke();
            }
            else
            {
                logger.Log("Navigating to " + otherFormScene.SceneName + ".", this);
                SceneManager.LoadScene(otherFormScene.SceneName);
            }
        });

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _messageError = ui.Q<Label>("MessageError");

        HideErrorMessage();
    }

    private void OnDisable()
    {
        if (_loginButton != null)
            _loginButton.clicked -= OnLoginClicked;
    }

    private async void OnLoginClicked()
    {
        logger.Log("Login Button Clicked", this);
        logger.Log("Email: " + _emailField.value, this);
        logger.Log("Password: " + _passwordField.value, this);

        HideErrorMessage();

        string err = await authService.Login(_emailField.value, _passwordField.value);

        if (!string.IsNullOrEmpty(err))
        {
            logger.Log("Login failed", this, Logging.LogType.Error);
            if (err == "You must verify your email address before you can log in.")
            {
                session.SetEmail(_emailField.value);
                session.SetPassword(_passwordField.value);
                if (OnNavigateToVerifyCode != null)
                {
                    logger.Log("Navigating to Verify Code for verification.", this);
                    OnNavigateToVerifyCode.Invoke();
                }
                else
                {
                    logger.Log(
                        "Navigating to " + verifyCodeScene.SceneName + " for verification.",
                        this
                    );
                    SceneManager.LoadScene(verifyCodeScene.SceneName);
                }
            }
            ShowErrorMessage(err);
            return;
        }
        if (OnLoginSuccess != null)
            OnLoginSuccess.Invoke();
        else
            SceneManager.LoadScene(targetScene.SceneName);
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
                    || e.ToLower().Contains("email") => "Wrong credentials.",
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
