using Application;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        // 1. Check if file exists to avoid crashing
        if (!System.IO.File.Exists("users.json"))
        {
            return Problem("Database file 'users.json' not found on server.");
        }

        // 2. Read file
        var json = await System.IO.File.ReadAllTextAsync("users.json");

        // 3. Handle empty file case
        if (string.IsNullOrWhiteSpace(json)) return Ok(new List<User>());

        var users = JsonSerializer.Deserialize<List<User>>(json);

        // 4. Default to empty list if search term is empty
        if (string.IsNullOrWhiteSpace(term)) return Ok(new List<User>());

        var results = users?
            .Where(u => u.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(results);
    }
}