using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MaziStore.ApiServer.Home.Extensions.Identity
{
    public static class CookieAuthEventsExtensions
    {
        public static void DisableRedirectionForApiClients(
           this CookieAuthenticationEvents authEvents
        )
        {
            authEvents.OnRedirectToLogin = redirCtx =>
               SelectiveRedirect(redirCtx, StatusCodes.Status401Unauthorized);

            authEvents.OnRedirectToAccessDenied = redirCtx =>
               SelectiveRedirect(redirCtx, StatusCodes.Status403Forbidden);

            authEvents.OnRedirectToLogout = redirCtx =>
               SelectiveRedirect(redirCtx, StatusCodes.Status200OK);

            authEvents.OnRedirectToReturnUrl = redirCtx =>
               SelectiveRedirect(redirCtx, StatusCodes.Status200OK);
        }

        private static Task SelectiveRedirect(
           RedirectContext<CookieAuthenticationOptions> redirectContext,
           int code
        )
        {
            if (IsApiRequest(redirectContext.Request))
            {
                redirectContext.Response.StatusCode = code;
                redirectContext.Response.Headers["Location"] =
                   redirectContext.RedirectUri;
            }
            return Task.CompletedTask;
        }

        private static bool IsApiRequest(HttpRequest httpRequest)
        {
            return httpRequest.Path.StartsWithSegments("/api");
        }
    }
}
