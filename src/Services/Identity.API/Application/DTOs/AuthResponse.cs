namespace Identity.API.Application.DTOs;

public class AuthResponse
{
    public string Token { get; init; }
    public int ExpiresIn { get; init; }
}