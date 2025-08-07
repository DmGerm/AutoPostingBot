// Ignore Spelling: Repo

using AutoPost_Bot.Models;
using AutoPost_Bot.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AutoPost_Bot.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUsersRepo _usersRepo;

        public LoginModel(IUsersRepo usersRepo)
        {
            _usersRepo = usersRepo;
        }

        [BindProperty]
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,}$",
        ErrorMessage = "Пароль должен содержать минимум 8 символов, заглавную букву и цифру.")]
        public string Password { get; set; } = "";

        public string? Error { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Error = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Page();
            }

            try
            {
                if (!await _usersRepo.IfAnyUsersAsync())
                    await _usersRepo.CreateUserAsync(Email, Password, RoleId.Root);

                var user = await _usersRepo.LoginUserAsync(Email, Password);

                if (user == null)
                {
                    Error = "Неверный email или пароль.";
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Email ?? string.Empty),
                    new(ClaimTypes.Role, user.RoleId.ToString())
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return Redirect("/");
            }
            catch (Exception ex)
            {
                Error = "Ошибка входа: " + ex.Message;
                return Page();
            }
        }
    }
}