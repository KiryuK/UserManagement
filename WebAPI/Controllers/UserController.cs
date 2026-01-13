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
        if (!System.IO.File.Exists("users.json")) return Problem("Database missing.");

        var json = await System.IO.File.ReadAllTextAsync("users.json");
        if (string.IsNullOrWhiteSpace(json)) return Ok(new List<User>());

        var users = JsonSerializer.Deserialize<List<User>>(json);
        if (string.IsNullOrWhiteSpace(term)) return Ok(new List<User>());

        var results = users?
            .Where(u =>
                u.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.IdentificationNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(results);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!System.IO.File.Exists("users.json")) return NotFound();

        var json = await System.IO.File.ReadAllTextAsync("users.json");
        var users = JsonSerializer.Deserialize<List<User>>(json);

        var user = users?.FirstOrDefault(u => u.Id == id);

        if (user == null) return NotFound();

        return Ok(user);
    }
}
