using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;
using Tutorial9.Model;
using Tutorial9.Services;


namespace Tutorial9.Controllers
{
  
  [Route("api/[controller]")] 
  [ApiController]
  public class WarehouseController: ControllerBase
  {
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
      _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> DoRequestAsync([FromBody] RequestDTO request)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var id = await _dbService.DoRequestAsync(request);
        return Ok(new { id = id });
      }
      catch (ProductNotFoundException ex)
      {
        return NotFound(new { error = ex.Message });
      }
      catch (WarehouseNotFoundException ex)
      {
        return NotFound(new { error = ex.Message });
      }
      catch (InvalidAmountException ex)
      {
        return BadRequest(new { error = ex.Message });
      }
      catch (OrderNotFoundException ex)
      {
        return NotFound(new { error = ex.Message });
      }
      catch (OrderAlreadyFulfilledException ex)
      {
        return Conflict(new { error = ex.Message });
      }
      // catch (Exception ex)
      // {
      //   return StatusCode(500, new { error = "Unexpected error", details = ex.Message });
      // }
    }
    
    
    
    [HttpPost("procedure")]
    public async Task<IActionResult> AddViaProcedure([FromBody] RequestDTO request)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      try
      {
        var id = await _dbService.DoRequestProcedureAsync(request);
        return Ok(new { IdProductWarehouse = id });
      }
      catch (SqlException ex)
      {
        return StatusCode(500, new { error = "Database error", details = ex.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = "Unexpected error", details = ex.Message });
      }
    }
    
  }
  
  
}
