namespace CreditFlow.Web.Core.Navigation;

public class PageHeaderService
{
    public string? Title { get; private set; }
    public string? Subtitle { get; private set; }

    public event Action? Changed;

    public void Set(string? title, string? subtitle = null)
    {
        Title = title;
        Subtitle = subtitle;
        Changed?.Invoke();
    }

    public void Clear() => Set(null, null);
}
