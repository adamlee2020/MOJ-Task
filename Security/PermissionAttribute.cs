using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MOJ_Task.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _code;
        public PermissionAttribute(string code) => _code = code;

        public Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
        {
            var user = ctx.HttpContext.User;
            if (!(user?.Identity?.IsAuthenticated ?? false))
            {
                ctx.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return Task.CompletedTask;
            }
            if (_code=="All")
            {
                return Task.CompletedTask;
            }
            var has = user.Claims.Any(c => c.Type == "perm" && c.Value == _code);
            if (!has) ctx.Result = new RedirectToActionResult("Denied", "Account", null);
            return Task.CompletedTask;
        }
    }
}
