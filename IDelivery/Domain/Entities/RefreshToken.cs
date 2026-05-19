namespace IDelivery.Domain;

public class RefreshToken : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}
