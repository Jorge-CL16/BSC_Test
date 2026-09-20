using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BSC.BusinessLogic.Models;
using Microsoft.IdentityModel.Tokens;

namespace BSC.Api.Auth;

/*
 Servicio responsable de generar tokens de acceso
 */
public sealed class JwtTokenService(IConfiguration configuration)
{
    public string CreateToken(UserDto user)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("La clave JWT no está configurada.");
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expirationMinutes = configuration.GetValue("Jwt:ExpirationMinutes", 60);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
