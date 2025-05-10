namespace Tutorial9.Exceptions;

public class OrderAlreadyFulfilledException : Exception
{
    public OrderAlreadyFulfilledException(int orderId)
        : base($"Order with ID {orderId} has already been fulfilled.") { }
}
