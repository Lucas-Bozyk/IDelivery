using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IDelivery.Application.DTOs.Auth;
using IDelivery.Application.IServices;
using IDelivery.Domain;
using IDelivery.Domain.ValueObjects;
using IDelivery.Infrastructure.Security;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IDelivery.Application.Services;

public class AuthService(IdentityDbContext db, DeliveryDbContext deliveryDb, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var exists = await db.Users.AnyAsync(x => x.Email == request.Email, ct);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var roleEnum = Enum.TryParse<UserRole>(request.Role, true, out var parsedRole) ? parsedRole : UserRole.Customer;
        var role = await db.Roles.FirstOrDefaultAsync(x => x.Name == roleEnum, ct);
        if (role is null)
        {
            role = new Role { Name = roleEnum };
            db.Roles.Add(role);
            await db.SaveChangesAsync(ct);
        }

        var user = new User { Email = request.Email.Trim().ToLowerInvariant(), PasswordHash = PasswordHasher.HashPassword(request.Password) };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        db.UserRoles.Add(new UserRoleMap { UserId = user.Id, RoleId = role.Id });
        var refresh = new RefreshToken { UserId = user.Id, Token = Guid.NewGuid().ToString("N"), ExpiresAt = DateTime.UtcNow.AddDays(7) };
        db.RefreshTokens.Add(refresh);
        await db.SaveChangesAsync(ct);

        if (roleEnum == UserRole.Customer)
        {
            deliveryDb.Customers.Add(new Customer
            {
                UserId = user.Id,
                FullName = request.Email.Split('@')[0],
                Phone = new PhoneNumber("11999999999").Value
            });
            await deliveryDb.SaveChangesAsync(ct);
        }

        var roles = new[] { role.Name.ToString() };
        return new AuthResponseDto(GenerateJwt(user, roles), refresh.Token);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new InvalidOperationException("Invalid credentials.");
        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash)) throw new InvalidOperationException("Invalid credentials.");

        var roles = await db.UserRoles.Where(x => x.UserId == user.Id).Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name.ToString()).ToListAsync(ct);
        if (roles.Count == 0) roles.Add(UserRole.Customer.ToString());

        var refresh = new RefreshToken { UserId = user.Id, Token = Guid.NewGuid().ToString("N"), ExpiresAt = DateTime.UtcNow.AddDays(7) };
        db.RefreshTokens.Add(refresh);
        await db.SaveChangesAsync(ct);
        return new AuthResponseDto(GenerateJwt(user, roles), refresh.Token);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
    {
        var refresh = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken, ct)
            ?? throw new InvalidOperationException("Invalid refresh token.");
        if (refresh.ExpiresAt < DateTime.UtcNow) throw new InvalidOperationException("Refresh token expired.");

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == refresh.UserId, ct)
            ?? throw new InvalidOperationException("User not found.");
        var roles = await db.UserRoles.Where(x => x.UserId == user.Id).Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name.ToString()).ToListAsync(ct);
        if (roles.Count == 0) roles.Add(UserRole.Customer.ToString());

        refresh.Token = Guid.NewGuid().ToString("N");
        refresh.ExpiresAt = DateTime.UtcNow.AddDays(7);
        await db.SaveChangesAsync(ct);
        return new AuthResponseDto(GenerateJwt(user, roles), refresh.Token);
    }

    private string GenerateJwt(User user, IEnumerable<string> roles)
    {
        var jwt = configuration.GetSection("Jwt");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
