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

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        logger.Log("VerifyCodeController enabled.", this);

        _verifyCodeButton = ui.Q<Button>("VerifyCodeButton");
        _verifyCodeButton.clicked += OnLoginClicked;

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
}
