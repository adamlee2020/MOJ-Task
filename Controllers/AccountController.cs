using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOJTaskDemo.Models.DTOs;
using MOJTaskDemo.Services;
using System.Security.Claims;

namespace MOJ_Task.Controllers
{
    public class AccountController(AuthService auth) : Controller
    {
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(dto);

            var user = await auth.AuthenticateAsync(dto, ct);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(dto);
            }

            // Claims
            var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new(ClaimTypes.Name, user.Username),
                            new("FullName", user.FullName),
                            new("Role", user.Roles[0])
                        };
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(user.Permissions.Select(p => new Claim("perm", p)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = dto.RememberMe, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

            // update last login
            await auth.SetLastLoginAsync(user.Id, ct);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Denied() => View();

    }
}
