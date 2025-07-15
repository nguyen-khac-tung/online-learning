using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Online_Learning.Repositories.Interfaces;
using System.Security.Claims;

namespace Online_Learning.Attributes
{

    public class DynamicAuthorize : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRoles = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!userRoles.Any())
            {
                context.Result = new ForbidResult();
                return;
            }

            var requestPath = context.HttpContext.Request.Path.Value?.ToLower();
            if (string.IsNullOrEmpty(requestPath))
            {
                context.Result = new ForbidResult();
                return;
            }

            var functionRepository = context.HttpContext.RequestServices.GetService<IFunctionRepository>();
            var allowedApiUrls = await functionRepository.GetAllowedApiUrlsForRolesAsync(userRoles);

            bool isAuthorized = allowedApiUrls.Any(baseUrl => requestPath.StartsWith(baseUrl));
            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
