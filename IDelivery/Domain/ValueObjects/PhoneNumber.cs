namespace IDelivery.Domain.ValueObjects;

public readonly record struct PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length < 10 || digits.Length > 13) throw new ArgumentException("Invalid phone number length.");
        Value = digits;
    }

    public override string ToString() => Value;
}
