using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MOJ_Task.Security;
using MOJTaskDemo.Models.DTOs;
using MOJTaskDemo.Models.Entities;
using MOJTaskDemo.Services;
using System.Security.Claims;

namespace MOJ_Task.Controllers
{
    public class ProfileController(ProfileService service) : Controller
    {

        [Permission("PROFILE_VIEW")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId)) return Unauthorized();

            var dto = await service.GetCurrentAsync(userId, ct);
            if (dto is null) return NotFound();

            return View(dto);
        }

    }
}
