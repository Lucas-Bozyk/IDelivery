using System.ComponentModel.DataAnnotations.Schema;
using IDelivery.Domain.ValueObjects;

namespace IDelivery.Domain;

public class Customer : EntityBase
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Cpf { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<CustomerAddress> Addresses { get; set; } = [];

    [NotMapped]
    public PhoneNumber PhoneNumber
    {
        get => new(Phone);
        set => Phone = value.Value;
    }

    [NotMapped]
    public Cpf? CpfValue
    {
        get => string.IsNullOrWhiteSpace(Cpf) ? null : new Cpf(Cpf);
        set => Cpf = value?.Value;
    }
}
