namespace IDelivery.Domain.ValueObjects;

public readonly record struct AddressVo(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode)
{
    public AddressVo Validate()
    {
        if (string.IsNullOrWhiteSpace(Street)) throw new ArgumentException("Street is required.");
        if (string.IsNullOrWhiteSpace(Number)) throw new ArgumentException("Number is required.");
        if (string.IsNullOrWhiteSpace(Neighborhood)) throw new ArgumentException("Neighborhood is required.");
        if (string.IsNullOrWhiteSpace(City)) throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(State) || State.Trim().Length != 2) throw new ArgumentException("State must have 2 chars.");
        var zipDigits = new string((ZipCode ?? string.Empty).Where(char.IsDigit).ToArray());
        if (zipDigits.Length != 8) throw new ArgumentException("ZipCode must have 8 digits.");
        return this with { ZipCode = zipDigits, State = State.Trim().ToUpperInvariant() };
    }
}
