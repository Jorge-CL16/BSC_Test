using BSC.Api.Auth;
using BSC.BusinessLogic.Models;
using BSC.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSC.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(
    IUserService userService,
    JwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<object>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.AuthenticateAsync(request, cancellationToken);

        if (user is null)
            return Unauthorized(new { message = "El correo electrónico o la contraseña no son válidos." });

        return Ok(new
        {
            token = jwtTokenService.CreateToken(user),
            user
        });
    }
}
