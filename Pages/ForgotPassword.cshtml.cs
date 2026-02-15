using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SuperUU.Data;
using SuperUU.Dto;

namespace SuperUU.Pages;

public class ForgotPasswordModel : PageModel
{
    private readonly AppDbContext _db;

    public ForgotPasswordModel(AppDbContext db)
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

        var email = Input.Email.Trim();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user != null && user.Status == 1)
        {
            var token = GenerateToken();
            var tokenHash = HashToken(token);
            var now = DateTime.Now;

            _db.UserPasswordResets.Add(new UserPasswordReset
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = now.AddMinutes(30),
                UsedAt = null,
                RequestIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
                CreatedAt = now
            });

            await _db.SaveChangesAsync();

            var link = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { token, email = user.Email },
                protocol: Request.Scheme);

            TempData["ResetLink"] = link;
        }

        TempData["Info"] = "若此信箱存在，我們已產生重設連結。";
        return RedirectToPage();
    }

    private static string GenerateToken()
    {
        var data = new byte[32];
        RandomNumberGenerator.Fill(data);
        return WebEncoders.Base64UrlEncode(data);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public class InputModel
    {
        [Display(Name = "電子郵件")]
        [Required(ErrorMessage = "請輸入電子郵件。")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確。")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}
