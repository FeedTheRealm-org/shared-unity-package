using System;

public interface IConfirmUI
{
    void Show(
        string question,
        Action onConfirm,
        Action onCancel = null,
        string title = "Confirm Action",
        string confirmText = null,
        string cancelText = null
    );

    void Hide();
}
