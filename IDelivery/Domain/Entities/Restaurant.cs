using System.ComponentModel.DataAnnotations.Schema;
using IDelivery.Domain.ValueObjects;

namespace IDelivery.Domain;

public class Restaurant : EntityBase
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsOpen { get; set; } = true;
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public Cnpj CnpjValue
    {
        get => new(Cnpj);
        set => Cnpj = value.Value;
    }

    [NotMapped]
    public PhoneNumber PhoneNumber
    {
        get => new(Phone);
        set => Phone = value.Value;
    }

    [NotMapped]
    public EmailAddress EmailValue
    {
        get => new(Email);
        set => Email = value.Value;
    }
}
