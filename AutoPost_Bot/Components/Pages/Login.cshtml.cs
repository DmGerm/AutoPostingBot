using AutoPost_Bot.Models;
using AutoPost_Bot.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
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
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public bool IsEmailInvalid { get; set; }
        public bool IsLoginError { get; set; }
        public string LoginErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            ValidateEmail();

            if (IsEmailInvalid)
                return Page();

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

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                IsLoginError = true;
                LoginErrorMessage = ex.Message;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
        }

        private void ValidateEmail()
        {
            IsEmailInvalid = string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
