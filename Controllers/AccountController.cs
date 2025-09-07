using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OrderingSystem.Data;

namespace OrderingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(UserRepository userRepository) { _userRepository = userRepository; }

        [HttpGet]
        public IActionResult Login(string? returnUrl) 
        { 
            ViewBag.ReturnUrl = returnUrl; 
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl)
        {
            var pwHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password ?? "")));
            
            var isValidUser= _userRepository.Validate(username, pwHash);
            if (isValidUser)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                };

                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
