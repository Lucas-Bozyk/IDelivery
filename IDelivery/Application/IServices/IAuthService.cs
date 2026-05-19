using IDelivery.Application.DTOs.Auth;

namespace IDelivery.Application.IServices;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken ct = default);
}
