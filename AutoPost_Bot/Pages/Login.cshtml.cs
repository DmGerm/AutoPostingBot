using AutoPost_Bot.Models;
using AutoPost_Bot.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? Error { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "¬ведите корректные данные";
                return Page();
            }

            try
            {
                if (!await _usersRepo.IfAnyUsersAsync())
                    await _usersRepo.CreateUserAsync(Email, Password, RoleId.Root);

                var user = await _usersRepo.LoginUserAsync(Email, Password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return Redirect("/");
            }
            catch (Exception ex)
            {
                Error = "ќшибка входа: " + ex.Message;
                return Page();
            }
        }
    }
}
