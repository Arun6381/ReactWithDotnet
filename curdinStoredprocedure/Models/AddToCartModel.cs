namespace curdinStoredprocedure.Models
{
    public class AddToCartModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Status { get; set; } 
    }

}
