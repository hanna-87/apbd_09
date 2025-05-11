using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;
using Tutorial9.Model;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;
    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
  

    public async Task<int> DoRequestAsync(RequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("WarehouseDatabase"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
         {
            //1
            command.CommandText = "SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct";
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            if ((int)await command.ExecuteScalarAsync() == 0)
            {
                throw new ProductNotFoundException(request.IdProduct);
            }

            
            command.Parameters.Clear();
            command.CommandText = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
            if ((int)await command.ExecuteScalarAsync() == 0)
            {
                throw new WarehouseNotFoundException(request.IdWarehouse);
            }

            if (request.Amount <= 0)
            {
                throw new InvalidAmountException(request.Amount);
            }
            
            //2
            Console.WriteLine($"Incoming CreatedAt: {request.CreatedAt:O} (Kind: {request.CreatedAt.Kind})");

            command.Parameters.Clear();
            command.CommandText = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct and Amount = @Amount ";
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);

            command.Parameters.AddWithValue("@Amount", request.Amount);
            int idOrder;
            await using (var reader = await command.ExecuteReaderAsync())
            {
                 if (! await reader.ReadAsync())
                            {
                                throw new OrderNotFoundException(request.IdProduct, request.Amount);
                            }
                 idOrder = reader.GetInt32(reader.GetOrdinal("IdOrder"));
                  
            }

            command.Parameters.Clear();
            command.CommandText = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct and Amount = @Amount and CreatedAt < @CreatedAt";
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
            command.Parameters.AddWithValue("@Amount", request.Amount);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                 if (! await reader.ReadAsync())
                            {
                                throw new     InvalidDateException() ;                                                               
                            }
                 idOrder = reader.GetInt32(reader.GetOrdinal("IdOrder"));
                  
            }
           
             //3
            command.Parameters.Clear();
            command.CommandText = "SELECT COUNT(*) FROM Product_Warehouse Where IdOrder = @IdOrder";
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            if (!((int)await command.ExecuteScalarAsync() == 0))
            {
                throw new OrderAlreadyFulfilledException(idOrder);
            }
            Console.WriteLine(idOrder);
            
            //4
            command.Parameters.Clear();
            command.CommandText = "UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
            command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            await command.ExecuteNonQueryAsync();
            
            //5
            command.Parameters.Clear();
            command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            var price = await command.ExecuteScalarAsync();
            
            var totalPrice = request.Amount*(decimal)price;
            
            command.Parameters.Clear();
            command.CommandText = @"INSERT INTO PRoduct_Warehouse (IdWarehouse,IdProduct, IdOrder, Amount, Price,CreatedAt)
OUTPUT INSERTED.IdProductwarehouse
VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount,@Price,  @CreatedAt);";
            command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", request.Amount);
            command.Parameters.AddWithValue("@Price", totalPrice);
            command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
            
            var finalId = await command.ExecuteScalarAsync();
            await transaction.CommitAsync();
            return Convert.ToInt32(finalId);

        }
         catch (Exception e)
         {
             await transaction.RollbackAsync();
             Console.WriteLine($"Transaction failed: {e.Message}");
             throw;
         }
    }


    public async Task<int> DoRequestProcedureAsync(RequestDTO request)
    {
              await using SqlConnection connection =
                new SqlConnection(_configuration.GetConnectionString("WarehouseDatabase"));
            await using SqlCommand command = new SqlCommand();
            command.Connection = connection;
            await connection.OpenAsync();
            command.CommandText = "AddProductToWarehouse";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", request.Amount);
            command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
            var result = await command.ExecuteScalarAsync();
            
            return Convert.ToInt32(result);
       
    }
}