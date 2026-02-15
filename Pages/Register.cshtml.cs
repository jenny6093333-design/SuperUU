using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SuperUU.Data;
using SuperUU.Dto;

namespace SuperUU.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public RegisterModel(AppDbContext db)
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

        var usernameExists = await _db.Users.AnyAsync(u => u.Username == Input.Username);
        if (usernameExists)
        {
            ModelState.AddModelError(nameof(Input.Username), "使用者名稱已被使用。");
            return Page();
        }

        var emailExists = await _db.Users.AnyAsync(u => u.Email == Input.Email);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(Input.Email), "電子郵件已被使用。");
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(Input.Gender))
        {
            var genderValue = Input.Gender.Trim();
            if (genderValue is not ("male" or "female" or "other"))
            {
                ModelState.AddModelError(nameof(Input.Gender), "性別欄位不正確。");
                return Page();
            }
        }

        var now = DateTime.Now;

        var user = new User
        {
            Username = Input.Username.Trim(),
            Email = Input.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(Input.Phone) ? null : Input.Phone.Trim(),
            FullName = string.IsNullOrWhiteSpace(Input.FullName) ? null : Input.FullName.Trim(),
            Gender = string.IsNullOrWhiteSpace(Input.Gender) ? null : Input.Gender.Trim(),
            Birthday = Input.Birthday,
            Status = 1,
            Role = "user",
            LastLoginAt = null,
            LoginFailCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
        };
        var identity = new System.Security.Claims.ClaimsIdentity(
            claims,
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToPage("Dashboard");
    }

    public class InputModel
    {
        [Display(Name = "使用者名稱")]
        [Required(ErrorMessage = "請輸入使用者名稱。")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "使用者名稱長度需介於 3 到 50 字元。")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "電子郵件")]
        [Required(ErrorMessage = "請輸入電子郵件。")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確。")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "手機")]
        [Phone(ErrorMessage = "手機格式不正確。")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "姓名")]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Display(Name = "性別")]
        [StringLength(20)]
        public string? Gender { get; set; }

        [Display(Name = "生日")]
        public DateOnly? Birthday { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "請輸入密碼。")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "密碼長度需至少 8 字元。")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "確認密碼")]
        [Required(ErrorMessage = "請再次輸入密碼。")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "兩次密碼不一致。")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
