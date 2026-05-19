namespace IDelivery.Domain;

public class Review : EntityBase
{
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid OrderId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";

    public static Review Create(Guid customerId, Order order, int rating, string comment)
    {
        order.EnsureCanBeReviewedBy(customerId);
        return new Review
        {
            CustomerId = customerId,
            OrderId = order.Id,
            RestaurantId = order.RestaurantId,
            Rating = rating,
            Comment = comment
        };
    }
}
