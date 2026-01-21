using Application;
using System.Text.Json;

namespace WebAPI.Services;

public class FileUserService : IUserService
{
    private readonly string _filePath = "users.json";

    public async Task<List<User>> SearchAsync(string term)
    {
        if (!File.Exists(_filePath)) return new List<User>();
        var json = await File.ReadAllTextAsync(_filePath);
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new();

        if (string.IsNullOrWhiteSpace(term)) return new List<User>();

        return users
            .Where(u => u.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        u.IdentificationNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        if (!File.Exists(_filePath)) return null;
        var json = await File.ReadAllTextAsync(_filePath);
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new();

        return users.FirstOrDefault(u => u.Id == id);
    }
}