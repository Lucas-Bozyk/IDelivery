namespace IDelivery.Domain.ValueObjects;

public readonly record struct Cnpj
{
    public string Value { get; }

    public Cnpj(string value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length != 14) throw new ArgumentException("CNPJ must have 14 digits.");
        Value = digits;
    }

    public override string ToString() => Value;
}
