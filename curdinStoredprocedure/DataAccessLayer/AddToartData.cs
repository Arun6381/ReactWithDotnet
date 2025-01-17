using curdinStoredprocedure.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace curdinStoredprocedure.DataAccessLayer
{
    public class AddToartData
    {
        private readonly string? _connectionString;

        public AddToartData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> AddToCartProductAsync(int userId, int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("AddToCartProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@UserId", userId));
                command.Parameters.Add(new SqlParameter("@ProductId", productId));

                var result = await command.ExecuteScalarAsync();

                return result.ToString();
            }
        }

        // Method to get cart details with product and user information
        public async Task<List<AddToCartModel>> GetCartDetailsWithInfoAsync()
        {
            var cartDetails = new List<AddToCartModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                //var command = new SqlCommand("GetCartDetailsWithInfo", connection)
                var command = new SqlCommand("GetCartDetailsWith", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var cart = new AddToCartModel
                        {
                            CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                            Status = reader.GetInt32(reader.GetOrdinal("Status")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            FirstName = reader["FirstName"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            Price = (decimal)reader["Price"],
                        };

                        cartDetails.Add(cart);
                    }
                }
            }

            return cartDetails;
        }

        // Method to update cart status
        public string UpdateStatusToCompleted(int cartId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("UpdateStatusToCompleted", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@CartId", cartId));

                var result = command.ExecuteNonQuery(); // Use ExecuteNonQuery for updating data

                return result.ToString();
            }
        }

        public async Task<List<AddToCartModel>> GetCartDetailsByUserIdAsync(int userId)
        {
            var cartDetailsList = new List<AddToCartModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetCartDetailsByUserId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cartDetailsList.Add(new AddToCartModel
                            {
                                CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                                Status = reader.GetInt32(reader.GetOrdinal("Status")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }

            return cartDetailsList;
        }


        public async Task<int> DeleteCartItemAsync(int cartId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteCartItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CartId", cartId);

                    conn.Open();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
        }
        }
}
