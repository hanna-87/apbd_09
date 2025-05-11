using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tutorial9.Model;

public class RequestDTO
{
    
    [Required]
    [Range(1, int.MaxValue)]
    public int IdProduct { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int IdWarehouse { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }
    [Required]
    
    public DateTime CreatedAt { get; set; }
}