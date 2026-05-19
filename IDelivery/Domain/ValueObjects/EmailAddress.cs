namespace IDelivery.Domain.ValueObjects;

public readonly record struct EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        value = value?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@')) throw new ArgumentException("Invalid email.");
        Value = value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}
