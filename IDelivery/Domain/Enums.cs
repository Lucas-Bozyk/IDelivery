namespace IDelivery.Domain;

public enum OrderStatus { Created, Confirmed, Preparing, OutForDelivery, Completed, Cancelled }
public enum PaymentStatus { Pending, Approved, Rejected, Refunded }
public enum PaymentMethod { CreditCard, DebitCard, Pix, Cash }
public enum DeliveryStatus { Pending, Assigned, PickedUp, Delivered, Cancelled }
public enum DiscountType { Fixed, Percentage }
public enum UserRole { Admin, Customer, RestaurantOwner, DeliveryDriver }
