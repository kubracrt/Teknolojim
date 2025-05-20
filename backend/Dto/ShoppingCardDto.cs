public class ShoppingCardDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public decimal Price { get; set; }
    public string ProductName { get; set; }
    public int ProductId { get; set; }
    public string? ImageUrl { get; set; }
    public int quantity { get; set; }
    public int UserId { get; set; }
}