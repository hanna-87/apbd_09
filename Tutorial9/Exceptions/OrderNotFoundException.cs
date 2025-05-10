namespace Tutorial9.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(int productId, int amount)
        : base($"Order for product ID {productId} with amount {amount} was not found.") { }
}
