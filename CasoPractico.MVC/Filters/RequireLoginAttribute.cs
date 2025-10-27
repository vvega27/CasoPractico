using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasoPractico.MVC.Filters
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null) return;

            var path = context.HttpContext.Request.Path.Value?.ToLowerInvariant() ?? "";
            if (path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib")
                || path.StartsWith("/images") || path.StartsWith("/favicon")) return;

            var user = context.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null); 
            }
        }
    }
}
