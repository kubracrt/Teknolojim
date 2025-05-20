public class OrderDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Usermail { get; set; } = "";
    public int ProductId { get; set; }
    public int quantity { get; set; }
    public string ImageUrl { get; set; } = "";
    public string OrderNumber { get; set; } = "";
    public DateTime CreatedDate { get; set; }
}