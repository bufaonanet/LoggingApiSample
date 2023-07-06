using System.ComponentModel.DataAnnotations.Schema;

namespace LoggingApiSample.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "numeric(18,4)")]
    public decimal Price { get; set; }
}