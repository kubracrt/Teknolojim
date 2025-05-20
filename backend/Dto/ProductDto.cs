using Microsoft.EntityFrameworkCore;

[Keyless]

public class ProductDto
{
    public int Id { get; set; }
    public string UserName { get; set; } =string.Empty;
    public string Name { get; set; } =string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } =string.Empty;
    public int Stock { get; set; }
     public string CategoryName { get; set; } =string.Empty;

}