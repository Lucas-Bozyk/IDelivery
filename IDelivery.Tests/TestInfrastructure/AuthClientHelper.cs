using System.Net.Http.Headers;
using System.Net.Http.Json;
using IDelivery.Application.DTOs.Auth;

namespace IDelivery.Tests.TestInfrastructure;

public static class AuthClientHelper
{
    public static async Task<AuthResponseDto> RegisterAndLoginAsync(HttpClient client, string email, string role)
    {
        var register = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDto(email, "StrongPass#123", role));
        register.EnsureSuccessStatusCode();
        var payload = await register.Content.ReadFromJsonAsync<AuthResponseDto>();
        return payload!;
    }

    public static void SetBearer(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
