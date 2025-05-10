using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Tutorial9.Model;

public class RequestDTO
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}