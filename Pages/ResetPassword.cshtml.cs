using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperUU.Data;
using SuperUU.Dto;

namespace SuperUU.Pages;

public class ResetPasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public ResetPasswordModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IActionResult OnGet(string? token, string? email)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
        {
            return RedirectToPage("ForgotPassword");
        }

        Input.Token = token;
        Input.Email = email;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var email = Input.Email.Trim();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null || user.Status == 0)
        {
            ModelState.AddModelError(string.Empty, "重設連結無效。");
            return Page();
        }

        var tokenHash = HashToken(Input.Token);
        var now = DateTime.Now;
        var reset = await _db.UserPasswordResets
            .Where(r => r.UserId == user.Id && r.TokenHash == tokenHash && r.UsedAt == null && r.ExpiresAt >= now)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

        if (reset == null)
        {
            ModelState.AddModelError(string.Empty, "重設連結已失效。");
            return Page();
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);
        user.UpdatedAt = now;

        reset.UsedAt = now;

        await _db.SaveChangesAsync();

        TempData["Info"] = "密碼已更新，請重新登入。";
        return RedirectToPage("Login");
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public class InputModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Display(Name = "新密碼")]
        [Required(ErrorMessage = "請輸入新密碼。")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "密碼長度需至少 8 字元。")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "確認新密碼")]
        [Required(ErrorMessage = "請再次輸入新密碼。")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "兩次密碼不一致。")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
