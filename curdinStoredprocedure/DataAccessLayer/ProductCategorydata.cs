using curdinStoredprocedure.Models;
using Microsoft.Build.Evaluation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace curdinStoredprocedure.DataAccessLayer
{
    public class ProductCategoryData
    {
        private readonly string? _connectionString;

        public ProductCategoryData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int InsertProductCategory(string categoryName, string description)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertProductCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryName", categoryName);
                    command.Parameters.AddWithValue("@Description", description);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public void UpdateProductCategory(int categoryId, string categoryName, string description)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateProductCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Category_Id", categoryId);
                    command.Parameters.AddWithValue("@CategoryName", categoryName);
                    command.Parameters.AddWithValue("@Description", description);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        
        public void DeleteProductCategory(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteProductCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Category_Id", categoryId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public Productcategory GetProductCategoryById(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetProductCategoryById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Category_Id", categoryId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Productcategory
                            {
                                CategoryId = (int)reader["Category_Id"], // Ensure this matches the table column
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        //public List<Productcategory> GetAllProductCategories()
        //{
        //    var categories = new List<Productcategory>();

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand("GetAllProductCategories", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;

        //            connection.Open();
        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    // Check if the category already exists
        //                    var categoryId = (int)reader["Category_Id"];
        //                    var category = categories.FirstOrDefault(c => c.CategoryId == categoryId);

        //                    if (category == null)
        //                    {
        //                        // Add new category
        //                        category = new Productcategory
        //                        {
        //                            CategoryId = categoryId,
        //                            CategoryName = reader["CategoryName"].ToString(),
        //                            Description = reader["Description"].ToString(),
        //                            //ProductItems = new List<ProductItems>()
        //                        };
        //                        //categories.Add(category);
        //                    }

        //                    // Add related product if available
        //                    //if (reader["Product_Id"] != DBNull.Value)
        //                    //{
        //                    //    category.ProductItems.Add(new ProductItems
        //                    //    {
        //                    //        Product_Id = (int)reader["Product_Id"],
        //                    //        ProductName = reader["ProductName"].ToString(),
        //                    //        Price = (decimal)reader["Price"],
        //                    //        Description = reader["ProductDescription"].ToString()
        //                    //    });
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    return categories;
        //}
        public List<Productcategory> GetAllProductCategories()
        {
            var categories = new List<Productcategory>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllProductCategories", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Productcategory
                            {
                                CategoryId = (int)reader["Category_Id"],
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }

            return categories;
        }


    }
}
