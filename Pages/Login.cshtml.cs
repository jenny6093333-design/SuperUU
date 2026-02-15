using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperUU.Data;
using SuperUU.Dto;

namespace SuperUU.Pages;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var username = Input.Username.Trim();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
            return Page();
        }

        if (user.Status == 0)
        {
            ModelState.AddModelError(string.Empty, "帳號已停用。");
            return Page();
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
            return Page();
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);
        }

        user.LastLoginAt = DateTime.Now;
        user.LoginFailCount = 0;
        user.UpdatedAt = DateTime.Now;

        _db.UserLoginLogs.Add(new UserLoginLog
        {
            UserId = user.Id,
            Username = username,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent = Request.Headers.UserAgent.ToString(),
            LoginStatus = 1,
            FailReason = null,
            CreatedAt = DateTime.Now
        });

        await _db.SaveChangesAsync();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("Dashboard");
    }

    public class InputModel
    {
        [Display(Name = "使用者名稱")]
        [Required(ErrorMessage = "請輸入使用者名稱。")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "請輸入密碼。")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
