using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VerifyCodeController : MonoBehaviour
{
    [SerializeField]
    private API.AuthService authService;

    [SerializeField]
    private Session.Session session;

    [SerializeField]
    private SceneReference targetScene;

    [SerializeField]
    private SceneReference backScene;

    [SerializeField]
    private Logging.Logger logger;

    private VisualElement ui;

    private Button _verifyCodeButton;
    private TextField _emailField;
    private TextField _codeField;
    private Label _changeButton;
    private Label _messageError;
    private Label _refreshCodeButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        logger.Log("VerifyCodeController enabled.", this);

        _verifyCodeButton = ui.Q<Button>("VerifyCodeButton");
        _verifyCodeButton.clicked += OnLoginClicked;

        _refreshCodeButton = ui.Q<Label>("RefreshCodeButton");
        if (_refreshCodeButton != null)
            _refreshCodeButton.RegisterCallback<ClickEvent>(evt => OnRefreshCodeClicked());

        _changeButton = ui.Q<Label>("LoginBackButton");
        _changeButton = ui.Q<Label>("LoginBackButton");
        _changeButton.RegisterCallback<ClickEvent>(evt =>
        {
            logger.Log("Navigating to " + backScene.SceneName + ".", this);
            SceneManager.LoadScene(backScene.SceneName);
        });

        _codeField = ui.Q<TextField>("CodeField");
        _messageError = ui.Q<Label>("MessageError");
    }

    private void OnDisable()
    {
        if (_verifyCodeButton != null)
            _verifyCodeButton.clicked -= OnLoginClicked;
        if (_refreshCodeButton != null)
            _refreshCodeButton.UnregisterCallback<ClickEvent>(evt => OnRefreshCodeClicked());
    }

    private async void OnLoginClicked()
    {
        logger.Log(
            $"Verify code Button Clicked - Email: {session.Email}, Code: {_codeField.value}",
            this
        );

        var (success, err) = await authService.VerifyCode(session.Email, _codeField.value);

        if (!success)
        {
            _messageError.text = err;
            return;
        }
        var loginErr = await authService.Login(session.Email, session.Password);
        if (string.IsNullOrEmpty(loginErr))
            SceneManager.LoadScene(targetScene.SceneName);
        else
            _messageError.text = loginErr;
    }

    private async void OnRefreshCodeClicked()
    {
        logger.Log($"Refresh code Button Clicked - Email: {session.Email}", this);

        var (success, err) = await authService.RefreshVerification(session.Email);

        if (success)
        {
            _messageError.text = "Sending you a new verification code.";
        }
        else
        {
            _messageError.text = err;
        }

        StartCoroutine(ClearMessageAfterSeconds(3f));
    }

    private IEnumerator ClearMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (_messageError != null)
            _messageError.text = "";
    }
}
