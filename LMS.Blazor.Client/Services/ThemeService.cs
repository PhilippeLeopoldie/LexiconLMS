using Microsoft.JSInterop;

namespace LMS.Blazor.Client.Services;
public class ThemeService(IJSRuntime jsRuntime)
{
    private const string LocalStorageKey = "theme";
    public string CurrentTheme { get; private set; } = "light";
    private bool _isInitialized = false;

    public event Action? OnChange;
    public async Task InitializeThemeAsync()
    {
        if (_isInitialized) return;
        var savedTheme = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
        if (!string.IsNullOrEmpty(savedTheme))
        {
            CurrentTheme = savedTheme;
        }
        await ApplyThemeAsync();
        _isInitialized = true;
    }

    public async Task SetTheme(string theme)
    {
        CurrentTheme = theme;
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, theme);
        await ApplyThemeAsync();
        await jsRuntime.InvokeVoidAsync("setTheme", theme);
        OnChange?.Invoke();
    }

    public async Task ApplyThemeAsync()
    {
        await jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-bs-theme", CurrentTheme);
    }
}
