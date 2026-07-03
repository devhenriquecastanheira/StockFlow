using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockFlow.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password != request.ConfirmPassword)
        {
            return false;
        }

        var existingUser = await _repository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return false;
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Password = request.Password,
            Role = request.Role
        };

        if (user.Role == UserRole.Cliente)
        {
            user.CustomerProfile = new CustomerProfile();
        }

        await _repository.AddUserAsync(user);

        return true;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _repository.AuthenticateAsync(request.Email, request.Password);
        if (user is null)
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddHours(2);

        return new AuthResponse
        {
            Token = GenerateJwtToken(user, expiresAt),
            ExpiresAt = expiresAt,
            Role = user.Role.ToString(),
            Name = user.Name
        };
    }

    private string GenerateJwtToken(User user, DateTime expiresAt)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
