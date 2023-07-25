using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BlazorClient.Server.Controllers;

public class AuthenticationController : Controller
{
    [HttpGet("~/login")]
    public ActionResult LogIn(string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            // Only allow local return URLs to prevent open redirect attacks.
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
        };

        // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
        return Challenge(properties);
    }

    [HttpPost("~/logout"), ValidateAntiForgeryToken]
    public async Task<ActionResult> LogOut(string returnUrl)
    {
        // Retrieve the identity stored in the local authentication cookie. If it's not available,
        // this indicate that the user is already logged out locally (or has not logged in yet).
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result is not { Succeeded: true })
        {
            // Only allow local return URLs to prevent open redirect attacks.
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
        }

        // Remove the local authentication cookie before triggering a redirection to the remote server.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties{
            // Only allow local return URLs to prevent open redirect attacks.
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
        };

        // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
        return SignOut(properties);
    }
    
}
