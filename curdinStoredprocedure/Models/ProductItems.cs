namespace curdinStoredprocedure.Models
{
    public class ProductItems
    {
        public int Product_Id { get; set; }
        public  string ProductName { get; set; }
        public decimal Price { get; set; }
        public  string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
