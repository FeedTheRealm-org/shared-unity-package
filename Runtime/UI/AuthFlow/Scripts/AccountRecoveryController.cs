using System.Collections;
using FTRShared.UI.AuthMenu;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class AccountRecoveryController : MonoBehaviour, IAuth
{
    private API.AuthService authService;
    private Session.Session session;
    private Logging.Logger logger;
    private AuthFlowManager flowManager;

    // Steps
    private VisualElement _stepEmail;
    private VisualElement _stepVerifyCode;
    private VisualElement _stepNewPassword;

    // Step 1
    private TextField _emailField;
    private Button _sendCodeButton;

    // Step 2
    private TextField _codeField;
    private Button _verifyCodeButton;
    private Button _resendCodeButton;

    // Step 3
    private TextField _newPasswordField;
    private TextField _repeatPasswordField;
    private Button _resetPasswordButton;

    // Shared per-step (each step has its own MessageError and BackToLoginButton)
    private Label _activeMessageError;
    private string _resetToken;

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
        var ui = GetComponent<UIDocument>().rootVisualElement;

        _stepEmail = ui.Q<VisualElement>("StepEmail");
        _stepVerifyCode = ui.Q<VisualElement>("StepVerifyCode");
        _stepNewPassword = ui.Q<VisualElement>("StepNewPassword");

        // Step 1
        _emailField = _stepEmail.Q<TextField>("EmailField");
        _sendCodeButton = _stepEmail.Q<Button>("SendCodeButton");
        _sendCodeButton.clicked += OnSendCodeClicked;
        _stepEmail.Q<Button>("BackToLoginButton").clicked += OnBackToLogin;

        // Step 2
        _codeField = _stepVerifyCode.Q<TextField>("CodeField");
        _verifyCodeButton = _stepVerifyCode.Q<Button>("VerifyCodeButton");
        _resendCodeButton = _stepVerifyCode.Q<Button>("ResendCodeButton");
        _verifyCodeButton.clicked += OnVerifyCodeClicked;
        _resendCodeButton.clicked += OnResendCodeClicked;
        _stepVerifyCode.Q<Button>("BackToLoginButton").clicked += OnBackToLogin;

        // Step 3
        _newPasswordField = _stepNewPassword.Q<TextField>("NewPasswordField");
        _repeatPasswordField = _stepNewPassword.Q<TextField>("RepeatPasswordField");
        _resetPasswordButton = _stepNewPassword.Q<Button>("ResetPasswordButton");
        _resetPasswordButton.clicked += OnResetPasswordClicked;
        _stepNewPassword.Q<Button>("BackToLoginButton").clicked += OnBackToLogin;

        // Close
        ui.Q<Button>("Close").clicked += OnBackToLogin;

        GoToStep(1);
    }

    private void OnDisable()
    {
        if (_sendCodeButton != null)
            _sendCodeButton.clicked -= OnSendCodeClicked;
        if (_verifyCodeButton != null)
            _verifyCodeButton.clicked -= OnVerifyCodeClicked;
        if (_resendCodeButton != null)
            _resendCodeButton.clicked -= OnResendCodeClicked;
        if (_resetPasswordButton != null)
            _resetPasswordButton.clicked -= OnResetPasswordClicked;
    }

    private void GoToStep(int step)
    {
        _stepEmail.style.display = step == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        _stepVerifyCode.style.display = step == 2 ? DisplayStyle.Flex : DisplayStyle.None;
        _stepNewPassword.style.display = step == 3 ? DisplayStyle.Flex : DisplayStyle.None;

        _activeMessageError = step switch
        {
            1 => _stepEmail.Q<Label>("MessageError"),
            2 => _stepVerifyCode.Q<Label>("MessageError"),
            3 => _stepNewPassword.Q<Label>("MessageError"),
            _ => null,
        };

        HideError();
    }

    private void OnBackToLogin()
    {
        logger.Log("Navigating back to Login.", this);
        flowManager.ShowPanel(AuthPanel.Login);
    }

    private async void OnSendCodeClicked()
    {
        logger.Log($"Sending recovery code to: {_emailField.value}", this);
        HideError();

        var (success, message) = await authService.ForgotPassword(_emailField.value);

        if (!success)
        {
            ShowError(message);
            return;
        }

        session.SetEmail(_emailField.value);
        logger.Log("Recovery code sent, moving to verify step.", this);
        GoToStep(2);
    }

    private async void OnVerifyCodeClicked()
    {
        logger.Log($"Verifying reset code for: {session.Email}", this);
        HideError();

        var (success, resetToken, message) = await authService.VerifyResetCode(
            session.Email,
            _codeField.value
        );

        if (!success)
        {
            ShowError(message);
            return;
        }

        _resetToken = resetToken;
        logger.Log("Reset code verified, moving to new password step.", this);
        GoToStep(3);
    }

    private async void OnResendCodeClicked()
    {
        logger.Log($"Resending recovery code to: {session.Email}", this);

        var (success, message) = await authService.ForgotPassword(session.Email);

        ShowError(success ? "A new code has been sent to your email." : message);
        StartCoroutine(ClearErrorAfterSeconds(3f));
    }

    private async void OnResetPasswordClicked()
    {
        logger.Log("Resetting password.", this);
        HideError();

        if (_newPasswordField.value != _repeatPasswordField.value)
        {
            ShowError("Passwords do not match.");
            return;
        }

        var (success, message) = await authService.ResetPassword(
            _resetToken,
            _newPasswordField.value
        );

        if (!success)
        {
            ShowError(message);
            return;
        }

        logger.Log("Password reset successful, navigating to Login.", this);
        flowManager.PasswordResetCompletion(
            "Password reset successful. Please log in with your new password."
        );
        flowManager.ShowPanel(AuthPanel.Login);
    }

    private void ShowError(string message)
    {
        if (_activeMessageError == null)
            return;
        _activeMessageError.text = message;
        _activeMessageError.style.display = DisplayStyle.Flex;
    }

    private void HideError()
    {
        if (_activeMessageError == null)
            return;
        _activeMessageError.text = "";
        _activeMessageError.style.display = DisplayStyle.None;
    }

    private IEnumerator ClearErrorAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideError();
    }
}
