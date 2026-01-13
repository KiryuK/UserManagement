using Application;

namespace Web.Services;

public class AppState
{
    public List<User> OpenUsers { get; private set; } = new();
    public event Action? OnChange;

    public void AddUser(User user)
    {
        if (!OpenUsers.Any(u => u.Id == user.Id))
        {
            OpenUsers.Add(user);
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}