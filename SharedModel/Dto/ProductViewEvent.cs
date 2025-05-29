namespace SharedModel.Dto
{
    public class ProductViewEvent
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public DateTime ViewedAt { get; set; }
    }

}