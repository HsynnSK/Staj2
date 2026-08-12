namespace BOMManagement.Services;

public enum ToastLevel
{
    Success,
    Error,
    Warning,
    Info
}

public class ToastMessage
{
    public string Message { get; set; } = string.Empty;
    public ToastLevel Level { get; set; }
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class ToastService
{
    public event Action<ToastMessage>? OnShow;
    public event Action<string>? OnHide;

    public void ShowSuccess(string message) =>
        OnShow?.Invoke(new ToastMessage { Message = message, Level = ToastLevel.Success });

    public void ShowError(string message) =>
        OnShow?.Invoke(new ToastMessage { Message = message, Level = ToastLevel.Error });

    public void ShowWarning(string message) =>
        OnShow?.Invoke(new ToastMessage { Message = message, Level = ToastLevel.Warning });

    public void ShowInfo(string message) =>
        OnShow?.Invoke(new ToastMessage { Message = message, Level = ToastLevel.Info });

    public void Hide(string id) =>
        OnHide?.Invoke(id);
}
