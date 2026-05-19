using System.ComponentModel.DataAnnotations.Schema;
using IDelivery.Domain.ValueObjects;

namespace IDelivery.Domain;

public class CustomerAddress : EntityBase
{
    public Guid CustomerId { get; set; }
    public string Street { get; set; } = "";
    public string Number { get; set; } = "";
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public bool IsDefault { get; set; }

    [NotMapped]
    public AddressVo Address
    {
        get => new AddressVo(Street, Number, Complement, Neighborhood, City, State, ZipCode).Validate();
        set
        {
            var validated = value.Validate();
            Street = validated.Street;
            Number = validated.Number;
            Complement = validated.Complement;
            Neighborhood = validated.Neighborhood;
            City = validated.City;
            State = validated.State;
            ZipCode = validated.ZipCode;
        }
    }
}
