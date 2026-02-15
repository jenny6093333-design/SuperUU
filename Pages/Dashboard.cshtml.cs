using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SuperUU.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;

    public void OnGet()
    {
        Username = User.Identity?.Name ?? string.Empty;
        Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        DisplayName = string.IsNullOrWhiteSpace(Username) ? "會員" : Username;
    }
}
