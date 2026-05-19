namespace IDelivery.Domain;

public class Coupon : EntityBase
{
    public string Code { get; set; } = "";
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal MinValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }

    public decimal Apply(decimal subtotal)
    {
        if (ExpirationDate < DateTime.UtcNow || subtotal < MinValue || UsedCount >= UsageLimit)
            throw new InvalidOperationException("Cupom invalido por data, valor minimo ou limite de uso.");
        UsedCount += 1;
        return DiscountType == DiscountType.Fixed ? Value : subtotal * (Value / 100m);
    }
}
