using System.ComponentModel.DataAnnotations.Schema;
using IDelivery.Domain.ValueObjects;

namespace IDelivery.Domain;

public class User : EntityBase
{
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public Guid? CustomerId { get; set; }
    public Guid? DeliveryDriverId { get; set; }
    public List<UserRoleMap> UserRoles { get; set; } = [];
    public List<RefreshToken> RefreshTokens { get; set; } = [];

    [NotMapped]
    public EmailAddress EmailValue
    {
        get => new(Email);
        set => Email = value.Value;
    }
}
