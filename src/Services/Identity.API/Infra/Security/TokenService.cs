using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.Application.DTOs;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Infra.Security;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthResponse Generate(User user)
    {
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInSeconds"]!);
        var expiration = DateTime.UtcNow.AddSeconds(expiresIn);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: GetClaims(user),
            expires: expiration,
            signingCredentials: GetCredentials()
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = expiresIn
        };
    }

    private IEnumerable<Claim> GetClaims(User user) =>
    [
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(JwtRegisteredClaimNames.Name, user.Name),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    ];

    private SigningCredentials GetCredentials()
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!)
        );

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
}