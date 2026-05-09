using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SignUpController : MonoBehaviour, IAuthUIController
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [SerializeField]
    private Logging.Logger logger;

    public event Action OnNavigateToLogin;
    public event Action OnSignUpSuccess;

    private GameObject _backgroundInstance;
    private VisualElement ui;

    private Button _signUpButton;
    private TextField _emailField;
    private TextField _passwordField;
    private TextField _repeatedPasswordField;
    private Label _messageError;
    private Button _changeButton;

    [SerializeField]
    public bool showBackground = true;

    [SerializeField]
    private GameObject backgroundPrefab;

    public void SetBackground(GameObject bg) => _backgroundInstance = bg;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        logger.Log("SignUpController enabled.", this);

        ui = GetComponent<UIDocument>().rootVisualElement;

        _signUpButton = ui.Q<Button>("SignUpButton");
        _changeButton = ui.Q<Button>("LoginChangeButton");

        _signUpButton.clicked += OnSignUpClicked;
        _changeButton.clicked += NavigateToLogin;

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _repeatedPasswordField = ui.Q<TextField>("RepeatPasswordField");
        _messageError = ui.Q<Label>("MessageError");

        if (showBackground && _backgroundInstance == null)
        {
            _backgroundInstance = Instantiate(backgroundPrefab);
        }
    }

    private void OnDisable()
    {
        if (_signUpButton != null)
            _signUpButton.clicked -= OnSignUpClicked;

        if (_changeButton != null)
            _changeButton.clicked -= NavigateToLogin;

        if (showBackground && _backgroundInstance != null)
        {
            Destroy(_backgroundInstance);
            _backgroundInstance = null;
        }
    }

    private void NavigateToLogin()
    {
        logger.Log("Navigating to Login.", this);
        OnNavigateToLogin?.Invoke();
    }

    private async void OnSignUpClicked()
    {
        logger.Log("SignUp Button Clicked", this);
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
            OnSignUpSuccess?.Invoke();
        }
        else
        {
            logger.Log("SignUp failed", this, Logging.LogType.Error);
            _messageError.text = err;
        }
    }
}
