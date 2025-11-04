using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class VerifyCodeController : MonoBehaviour {
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

    private AsyncOperation preloadOperation;

    private void Awake() {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable() {
        logger.Log("VerifyCodeController enabled.", this);

        _verifyCodeButton = ui.Q<Button>("VerifyCodeButton");
        _verifyCodeButton.clicked += OnLoginClicked;

        _changeButton = ui.Q<Label>("LoginBackButton");
        _changeButton = ui.Q<Label>("LoginBackButton");
        _changeButton.RegisterCallback<ClickEvent>(evt => {
            logger.Log("Navigating to " + backScene.SceneName + ".", this);
            SceneManager.LoadScene(backScene.SceneName);
        });

        _emailField = ui.Q<TextField>("EmailField");
        _codeField = ui.Q<TextField>("CodeField");
        _messageError = ui.Q<Label>("MessageError");
    }

    private void OnDisable() {
        if (_verifyCodeButton != null)
            _verifyCodeButton.clicked -= OnLoginClicked;
    }

    private void OnLoginClicked() {
        logger.Log("Verify code Button Clicked", this);
        logger.Log("Email: " + _emailField.value, this);
        logger.Log("Code: " + _codeField.value, this);

        StartCoroutine(authService.VerifyCode(_emailField.value, _codeField.value, (success, err) => {
            if (success) {
                logger.Log("Verify code successful", this);
                SceneManager.LoadScene(targetScene.SceneName);
            } else {
                logger.Log("Verify code failed", this, Logging.LogType.Error);
                _messageError.text = err;
            }
        }));
    }
}
