using Application;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Web.Services;

public class AppState
{
    private readonly IJSRuntime _js;

    public AppState(IJSRuntime js)
    {
        _js = js;
    }

    public List<User> OpenUsers { get; private set; } = new();
    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", "openUsers");
            if (!string.IsNullOrEmpty(json))
            {
                OpenUsers = JsonSerializer.Deserialize<List<User>>(json) ?? new();
                NotifyStateChanged();
            }
        }
        catch { }
    }

    public async Task AddUser(User user)
    {
        if (!OpenUsers.Any(u => u.Id == user.Id))
        {
            if (OpenUsers.Count >= 8)
            {
                OpenUsers.RemoveAt(0);
            }

            OpenUsers.Add(user);

            await SaveState();
            NotifyStateChanged();
        }
    }

    public async Task RemoveUser(User user)
    {
        var item = OpenUsers.FirstOrDefault(u => u.Id == user.Id);
        if (item != null)
        {
            OpenUsers.Remove(item);
            await SaveState();
            NotifyStateChanged();
        }
    }

    private async Task SaveState()
    {
        var json = JsonSerializer.Serialize(OpenUsers);
        await _js.InvokeVoidAsync("localStorage.setItem", "openUsers", json);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}