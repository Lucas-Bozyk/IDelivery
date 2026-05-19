namespace IDelivery.Application.DTOs.Auth;

public record AuthResponseDto(string Token, string RefreshToken);
public record RegisterRequestDto(string Email, string Password, string Role = "Customer");
public record LoginRequestDto(string Email, string Password);
public record RefreshTokenRequestDto(string RefreshToken);
