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

    private VisualElement ui;

    private Button _loginButton;
    private TextField _emailField;
    private TextField _passwordField;
    private Label _changeButton;
    private Label _messageError;

    private AsyncOperation preloadOperation;

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
            logger.Log("Navigating to " + otherFormScene.SceneName + ".", this);
            SceneManager.LoadScene(otherFormScene.SceneName);
        });

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _messageError = ui.Q<Label>("MessageError");
    }

    private void OnDisable()
    {
        if (_loginButton != null)
            _loginButton.clicked -= OnLoginClicked;
    }

    private void OnLoginClicked()
    {
        logger.Log("Login Button Clicked", this);
        logger.Log("Email: " + _emailField.value, this);
        logger.Log("Password: " + _passwordField.value, this);

        StartCoroutine(
            authService.Login(
                _emailField.value,
                _passwordField.value,
                (err) =>
                {
                    if (string.IsNullOrEmpty(err))
                    {
                        logger.Log($"Navigating to {session.APIToken}", this);
                        if (preloadOperation != null)
                        {
                            preloadOperation.allowSceneActivation = true;
                            logger.Log("Activating preloaded " + targetScene.SceneName + ".", this);
                        }
                        else
                        {
                            SceneManager.LoadScene(targetScene.SceneName);
                        }
                    }
                    else
                    {
                        logger.Log("Login failed", this, Logging.LogType.Error);
                        if (err == "You must verify your email address before you can log in.")
                        {
                            logger.Log(
                                "Navigating to " + verifyCodeScene.SceneName + " for verification.",
                                this
                            );
                            session.SetEmail(_emailField.value);
                            session.SetPassword(_passwordField.value);
                            SceneManager.LoadScene(verifyCodeScene.SceneName);
                        }
                        _messageError.text = err;
                    }
                }
            )
        );
    }
}
