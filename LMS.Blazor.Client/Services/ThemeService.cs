using Microsoft.JSInterop;

namespace LMS.Blazor.Client.Services;
public class ThemeService(IJSRuntime jsRuntime)
{
    private const string LocalStorageKey = "theme";
    public string CurrentTheme { get; private set; } = "light";

    public event Action? OnChange;
    public async Task InitializeThemeAsync()
    {
        var savedTheme = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
        if (!string.IsNullOrEmpty(savedTheme))
        {
            CurrentTheme = savedTheme;
        }
        await ApplyThemeAsync();
    }

    public async Task SetTheme(string theme)
    {
        CurrentTheme = theme;
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, theme);
        await ApplyThemeAsync();
        OnChange?.Invoke();
    }

    private async Task ApplyThemeAsync()
    {
        await jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-bs-theme", CurrentTheme);
    }
}
