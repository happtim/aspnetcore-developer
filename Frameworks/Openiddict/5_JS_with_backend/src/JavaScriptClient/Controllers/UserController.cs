using System.Security.Claims;
using JavaScriptClient.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JavaScriptClient.Controllers;

// Original source: https://github.com/berhir/BlazorWebAssemblyCookieAuth.
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet("bff/user")]
    [AllowAnonymous]
    public IActionResult GetCurrentUser()
    {
        if (User.Identity.IsAuthenticated)
        {
            return  Ok(CreateUserInfo(User));
        }
        else
        {
            return Unauthorized();
        }
    }

    private static ICollection<ClaimValue> CreateUserInfo(ClaimsPrincipal principal)
    {
        List<ClaimValue> claims = new List<ClaimValue>();
    
        if (principal.Claims.Any())
        {

            foreach (var claim in principal.Claims)
            {
                claims.Add(new ClaimValue(claim.Type, claim.Value));
            }
        }

        return claims;
    }
}
