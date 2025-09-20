using Microsoft.AspNetCore.Mvc;
using MOJ_Task.Security;
using MOJTaskDemo.Models.DTOs;
using MOJTaskDemo.Services;

namespace MOJ_Task.Controllers
{
    [Permission("USER_MANAGE")]
    public class UsersController(UserService service) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var users = await service.GetUsersAsync(ct);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Roles = await service.GetAllRolesAsync(ct);
            return View(new UserEditDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserEditDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await service.GetAllRolesAsync(ct);
                return View(dto);
            }
            try
            {
                await service.CreateAsync(dto, ct);
                TempData["ok"] = "User created.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Roles = await service.GetAllRolesAsync(ct);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var dto = await service.GetByIdAsync(id, ct);
            if (dto is null) return NotFound();
            ViewBag.Roles = await service.GetAllRolesAsync(ct);
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await service.GetAllRolesAsync(ct);
                return View(dto);
            }
            try
            {
                await service.UpdateAsync(dto, ct);
                TempData["ok"] = "User updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Roles = await service.GetAllRolesAsync(ct);
                return View(dto);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await service.DeleteAsync(id, ct);
            TempData["ok"] = "User deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                TempData["err"] = "Password cannot be empty.";
                return RedirectToAction(nameof(Index));
            }
            await service.ResetPasswordAsync(dto, ct);
            TempData["ok"] = "Password reset.";
            return RedirectToAction(nameof(Index));
        }
    }
}
