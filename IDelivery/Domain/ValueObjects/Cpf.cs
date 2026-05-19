namespace IDelivery.Domain.ValueObjects;

public readonly record struct Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length != 11) throw new ArgumentException("CPF must have 11 digits.");
        Value = digits;
    }

    public override string ToString() => Value;
}
