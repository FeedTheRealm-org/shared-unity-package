using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour, IAuthUIController
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [SerializeField]
    private GameObject loginBackgroundPrefab;

    public bool showBackground = true;
    private GameObject _backgroundInstance;
    private bool _backgroundOwnershipTransferred = false;

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

    private void Start()
    {
        if (
            !_backgroundOwnershipTransferred
            && showBackground
            && loginBackgroundPrefab != null
            && _backgroundInstance == null
        )
            _backgroundInstance = Instantiate(loginBackgroundPrefab);
    }

    public void SetBackground(GameObject bg)
    {
        _backgroundInstance = bg;
        _backgroundOwnershipTransferred = true;
    }

    private void OnEnable()
    {
        logger.Log("LoginController enabled.", this);

        _loginButton = ui.Q<Button>("LoginButton");
        _loginButton.clicked += OnLoginClicked;

        _changeButton = ui.Q<Label>("SignUpChangeButton");
        _changeButton.RegisterCallback<ClickEvent>(evt =>
        {
            logger.Log("Navigating to Sign Up.", this);
            OnNavigateToSignUp?.Invoke();
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
            logger.Log("Login failed", this, Logging.LogType.Warning);
            if (err == "You must verify your email address before you can log in.")
            {
                session.SetEmail(_emailField.value);
                session.SetPassword(_passwordField.value);
                logger.Log("Navigating to Verify Code for verification.", this);
                OnNavigateToVerifyCode?.Invoke();
            }
            ShowErrorMessage(err);
            return;
        }
        OnLoginSuccess?.Invoke();
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
