using UnityEngine;
using UnityEngine.UIElements;

public class SignUpController : MonoBehaviour, IAuthUIController
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [Header("UI Prefabs")]
    [SerializeField]
    private GameObject loginUI;

    [SerializeField]
    private GameObject verifyCodeUI;

    [SerializeField]
    private Logging.Logger logger;

    private GameObject _backgroundInstance;
    private VisualElement ui;

    private Button _signUpButton;
    private TextField _emailField;
    private TextField _passwordField;
    private TextField _repeatedPasswordField;
    private Label _messageError;
    private Label _changeButton;

    public void SetBackground(GameObject bg) => _backgroundInstance = bg;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        logger.Log("SignUpController enabled.", this);

        _signUpButton = ui.Q<Button>("SignUpButton");
        _signUpButton.clicked += OnLoginClicked;

        _changeButton = ui.Q<Label>("LoginChangeButton");
        _changeButton.RegisterCallback<ClickEvent>(evt =>
        {
            logger.Log("Switching to Login UI.", this);
            SwitchTo(loginUI);
        });

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _repeatedPasswordField = ui.Q<TextField>("RepeatPasswordField");
        _messageError = ui.Q<Label>("MessageError");
    }

    private void OnDisable()
    {
        if (_signUpButton != null)
            _signUpButton.clicked -= OnLoginClicked;
    }

    private async void OnLoginClicked()
    {
        logger.Log("Login Button Clicked", this);
        logger.Log("Email: " + _emailField.value, this);
        logger.Log("Password: " + _passwordField.value, this);

        if (_passwordField.value != _repeatedPasswordField.value)
        {
            logger.Log("Passwords do not match", this, Logging.LogType.Warning);
            _messageError.text = "Passwords do not match.";
            return;
        }

        (bool success, string err) = await authService.SignUp(
            _emailField.value,
            _passwordField.value
        );

        if (success)
        {
            logger.Log("SignUp successful, email: " + _emailField.value, this);
            session.SetEmail(_emailField.value);
            session.SetPassword(_passwordField.value);
            SwitchTo(verifyCodeUI);
        }
        else
        {
            logger.Log("SignUp failed", this, Logging.LogType.Error);
            _messageError.text = err;
        }
    }

    private void SwitchTo(GameObject prefab)
    {
        if (prefab == null)
            return;
        var go = Instantiate(prefab);
        var controller = go.GetComponent<IAuthUIController>();
        if (controller != null)
        {
            controller.SetBackground(_backgroundInstance);
            _backgroundInstance = null;
        }
        else
        {
            Debug.LogWarning(
                $"Prefab '{prefab.name}' does not implement IAuthUIController. Background will not be handed off.",
                this
            );
        }
        Destroy(gameObject);
    }
}
