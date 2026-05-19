using System.Collections;
using FTRShared.UI.AuthMenu;
using UnityEngine;
using UnityEngine.UIElements;

public class VerifyCodeController : AuthController
{
    private VisualElement ui;
    private Button _verifyCodeButton;
    private Button _refreshCodeButton;
    private Button _loginBackButton;
    private Button _closeButton;
    private TextField _codeField;
    private Label _messageError;

    private void OnEnable()
    {
        logger.Log("VerifyCodeController enabled.", this);
        ui = GetComponent<UIDocument>().rootVisualElement;

        _verifyCodeButton = ui.Q<Button>("VerifyCodeButton");
        _refreshCodeButton = ui.Q<Button>("RefreshCodeButton");
        _loginBackButton = ui.Q<Button>("LoginBackButton");
        _closeButton = ui.Q<Button>("Close");

        _verifyCodeButton.clicked += OnVerifyClicked;
        _refreshCodeButton.clicked += OnRefreshCodeClicked;
        _loginBackButton.clicked += OnNavigateBack;
        _closeButton.clicked += OnClose;

        _codeField = ui.Q<TextField>("CodeField");
        _messageError = ui.Q<Label>("MessageError");

        HideErrorMessage();
    }

    private void OnDisable()
    {
        if (_verifyCodeButton != null)
            _verifyCodeButton.clicked -= OnVerifyClicked;
        if (_refreshCodeButton != null)
            _refreshCodeButton.clicked -= OnRefreshCodeClicked;
        if (_loginBackButton != null)
            _loginBackButton.clicked -= OnNavigateBack;
        if (_closeButton != null)
            _closeButton.clicked -= OnClose;
    }

    private void OnNavigateBack()
    {
        logger.Log("Navigating back to Login.", this);
        flowManager.ShowPanel(AuthPanel.Login);
    }

    private void OnClose()
    {
        logger.Log("Auth cancelled.", this);
        flowManager.CancelAuth();
    }

    private async void OnVerifyClicked()
    {
        logger.Log(
            $"Verify code Button Clicked - Email: {session.Email}, Code: {_codeField.value}",
            this
        );
        HideErrorMessage();

        var (success, err) = await authService.VerifyCode(session.Email, _codeField.value);

        if (!success)
        {
            ShowErrorMessage(err);
            return;
        }

        var loginErr = await authService.Login(session.Email, session.Password);
        if (string.IsNullOrEmpty(loginErr))
        {
            logger.Log("Verification and login successful.", this);
            flowManager.AuthCompletion("Verification successful.");
        }
        else
        {
            ShowErrorMessage(loginErr);
        }
    }

    private async void OnRefreshCodeClicked()
    {
        logger.Log($"Refresh code Button Clicked - Email: {session.Email}", this);

        var (success, err) = await authService.RefreshVerification(session.Email);

        ShowErrorMessage(success ? "Sending you a new verification code." : err);
        StartCoroutine(ClearMessageAfterSeconds(3f));
    }

    private IEnumerator ClearMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideErrorMessage();
    }

    private void ShowErrorMessage(string msg)
    {
        if (_messageError == null)
            return;
        _messageError.text = msg;
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
