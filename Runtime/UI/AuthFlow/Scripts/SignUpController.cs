using FTRShared.UI.AuthMenu;
using UnityEngine;
using UnityEngine.UIElements;

public class SignUpController : AuthController
{
    private VisualElement ui;
    private Button _signUpButton;
    private Button _loginChangeButton;
    private Button _closeButton;
    private TextField _emailField;
    private TextField _passwordField;
    private TextField _repeatedPasswordField;
    private Label _messageError;

    private void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        _signUpButton = ui.Q<Button>("SignUpButton");
        _loginChangeButton = ui.Q<Button>("LoginChangeButton");
        _closeButton = ui.Q<Button>("Close");

        _signUpButton.clicked += OnSignUpClicked;
        _loginChangeButton.clicked += OnNavigateToLogin;
        _closeButton.clicked += OnClose;

        _emailField = ui.Q<TextField>("EmailField");
        _passwordField = ui.Q<TextField>("PasswordField");
        _repeatedPasswordField = ui.Q<TextField>("RepeatPasswordField");
        _messageError = ui.Q<Label>("MessageError");

        HideErrorMessage();
    }

    private void OnDisable()
    {
        if (_signUpButton != null)
            _signUpButton.clicked -= OnSignUpClicked;
        if (_loginChangeButton != null)
            _loginChangeButton.clicked -= OnNavigateToLogin;
        if (_closeButton != null)
            _closeButton.clicked -= OnClose;
    }

    private void OnNavigateToLogin()
    {
        logger.Log("Navigating to Login.", this);
        flowManager.ShowPanel(AuthPanel.Login);
    }

    private void OnClose()
    {
        logger.Log("Auth cancelled.", this);
        flowManager.CancelAuth();
    }

    private async void OnSignUpClicked()
    {
        logger.Log("SignUp Button Clicked", this);
        HideErrorMessage();

        if (_passwordField.value != _repeatedPasswordField.value)
        {
            logger.Log("Passwords do not match", this, Logging.LogType.Warning);
            ShowErrorMessage("Passwords do not match.");
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
            flowManager.ShowPanel(AuthPanel.VerifyNewAccount);
        }
        else
        {
            logger.Log("SignUp failed", this, Logging.LogType.Error);
            ShowErrorMessage(err);
        }
    }

    private void ShowErrorMessage(string err)
    {
        if (_messageError == null)
            return;
        _messageError.text = err;
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
