namespace Tutorial9.Exceptions;

public class WarehouseNotFoundException : Exception
{
    public WarehouseNotFoundException(int warehouseId)
        : base($"Warehouse with ID {warehouseId} was not found.") { }
}
