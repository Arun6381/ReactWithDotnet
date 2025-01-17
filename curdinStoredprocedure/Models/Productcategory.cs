using Microsoft.Build.Evaluation;

namespace curdinStoredprocedure.Models
{
    public class Productcategory
    {
        public int CategoryId { get; set; }
        public  string CategoryName { get; set; }
        public  string Description { get; set; }
        //public  List<ProductItems> ProductItems { get; set; } // Navigation property

    }
}
