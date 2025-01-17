using Microsoft.Data.SqlClient;
using curdinStoredprocedure.Models;

using System.Data;
using Microsoft.Build.Evaluation;

namespace curdinStoredprocedure.DataAccessLayer
{
    public class ProductItemsData
    {
        private readonly string? _connectionString;

        public ProductItemsData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       

        public int InsertProductItem(ProductItems item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertProductItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ProductName", item.ProductName);
                    command.Parameters.AddWithValue("@Price", item.Price);
                    command.Parameters.AddWithValue("@Description", item.Description);
                    command.Parameters.AddWithValue("@CategoryId", item.CategoryId);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        //public void UpdateProductItem(ProductItems item)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand("UpdateProductItem", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                command.Parameters.AddWithValue("@Product_Id", item.Product_Id);
        //                command.Parameters.AddWithValue("@ProductName", (object)item.ProductName ?? DBNull.Value);
        //                command.Parameters.AddWithValue("@Price", (object)item.Price ?? DBNull.Value);
        //                command.Parameters.AddWithValue("@Description", (object)item.Description ?? DBNull.Value);
        //                command.Parameters.AddWithValue("@CategoryId", (object)item.CategoryId ?? DBNull.Value);

        //                connection.Open();
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (to console, file, etc.)
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //}
        public void UpdateProductItem(ProductItems item)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Build the SQL query dynamically based on non-null properties.
                    List<string> updateClauses = new List<string>();

                    if (!string.IsNullOrEmpty(item.ProductName))
                        updateClauses.Add("ProductName = @ProductName");
                    if (item.Price != null)
                        updateClauses.Add("Price = @Price");
                    if (!string.IsNullOrEmpty(item.Description))
                        updateClauses.Add("Description = @Description");
                    if (item.CategoryId != null)
                        updateClauses.Add("CategoryId = @CategoryId");

                    // Only execute the query if there's something to update.
                    if (updateClauses.Count > 0)
                    {
                        string updateQuery = $"UPDATE ProductItems SET {string.Join(", ", updateClauses)} WHERE Product_Id = @Product_Id";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Product_Id", item.Product_Id);

                            if (!string.IsNullOrEmpty(item.ProductName))
                                command.Parameters.AddWithValue("@ProductName", item.ProductName);
                            if (item.Price != null)
                                command.Parameters.AddWithValue("@Price", item.Price);
                            if (!string.IsNullOrEmpty(item.Description))
                                command.Parameters.AddWithValue("@Description", item.Description);
                            if (item.CategoryId != null)
                                command.Parameters.AddWithValue("@CategoryId", item.CategoryId);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteProductItem(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteProductItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Product_Id", productId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<ProductItems> GetAllProductItems()
        {
            var items = new List<ProductItems>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllProductItems", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        items.Add(new ProductItems
                        {
                            Product_Id = (int)reader["Product_Id"],
                            ProductName = reader["ProductName"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            CategoryId = (int)reader["CategoryId"]
                        });
                    }
                }
            }
            return items;
        }
        public ProductItems GetProductItemById(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetProductItemById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Product_Id", productId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new ProductItems
                        {
                            Product_Id = (int)reader["Product_Id"],
                            ProductName = reader["ProductName"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            CategoryId = (int)reader["CategoryId"]
                        };
                    }
                    return null;
                }
            }
        }
        public List<ProductItems> GetProductItemsByCategory(int categoryId)
        {
            var items = new List<ProductItems>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetProductItemsByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        items.Add(new ProductItems
                        {
                            Product_Id = (int)reader["Product_Id"],
                            ProductName = reader["ProductName"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            CategoryId = (int)reader["CategoryId"]
                        });
                    }
                }
            }
            return items;
        }
        public IEnumerable<Productcategory> GetAllProductCategories()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllProductCategorys", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var categories = new List<Productcategory>();
                        while (reader.Read())
                        {
                            categories.Add(new Productcategory
                            {
                                CategoryId = (int)reader["CategoryId"],
                                CategoryName = reader["CategoryName"].ToString()
                            });
                        }
                        return categories;
                    }
                }
            }
        }

    }
}
