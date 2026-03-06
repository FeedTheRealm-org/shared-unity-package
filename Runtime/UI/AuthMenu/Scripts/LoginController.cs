using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [Header("UI Prefabs")]
    [SerializeField]
    private GameObject signUpUI;

    [SerializeField]
    private GameObject verifyCodeUI;

    [SerializeField]
    private GameObject loginBackgroundPrefab;
    private GameObject loginBackgroundInstance;
    public bool showBackground = true;

    [SerializeField]
    private Logging.Logger logger;

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
            logger.Log("Switching to SignUp UI.", this);
            Destroy(gameObject);
            if (signUpUI != null)
                Instantiate(signUpUI);
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
                logger.Log("Switching to VerifyCode UI for verification.", this);
                session.SetEmail(_emailField.value);
                session.SetPassword(_passwordField.value);
                Destroy(gameObject);
                if (verifyCodeUI != null)
                    Instantiate(verifyCodeUI);
            }
            ShowErrorMessage(err);
            return;
        }
        logger.Log("Login successful, switching to main UI.", this);
        if (loginBackgroundInstance != null)
        {
            Destroy(loginBackgroundInstance);
            loginBackgroundInstance = null;
        }
        Destroy(gameObject);
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

    public void InitializeBackground(bool show)
    {
        showBackground = show;
        if (showBackground && loginBackgroundPrefab != null && loginBackgroundInstance == null)
        {
            loginBackgroundInstance = Instantiate(loginBackgroundPrefab);
            loginBackgroundInstance.SetActive(true);
        }
    }

    private void HideErrorMessage()
    {
        if (_messageError == null)
            return;
        _messageError.text = "";
        _messageError.style.display = DisplayStyle.None;
    }
}
