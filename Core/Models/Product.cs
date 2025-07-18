namespace Core.Models;

public class Product
{
    public required string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    public decimal Total => Quantity * Price;
}