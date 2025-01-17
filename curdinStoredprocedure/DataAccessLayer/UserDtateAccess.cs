using Microsoft.Data.SqlClient;
using curdinStoredprocedure.Models;

using System.Data;

namespace curdinStoredprocedure.DataAccessLayer
{
    public class UserDataAccess
    {
        private readonly string? _connectionString;

        public UserDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void InsertOAuthUser(string firstName, string lastName, string email, string oAuthProvider, string oAuthUserId, string phoneNumber = null)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertOAuthUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OAuthProvider", oAuthProvider);
                    cmd.Parameters.AddWithValue("@OAuthUserId", oAuthUserId);

                    if (string.IsNullOrEmpty(phoneNumber))
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            }


        // Check if an email exists
        public bool CheckEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("CheckEmailExists", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Insert a new user
        public void InsertUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("InsertUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", user.Gender);
                command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@Email", user.EmailAddress);
                command.Parameters.AddWithValue("@Username", user.Username);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Get user by email
        public UserModel GetUserByEmail(string email)
        {
            UserModel user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                user = new UserModel
                                {
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Roles=reader.GetString(reader.GetOrdinal("Roles")),
                                    FirstName=reader.GetString(reader.GetOrdinal("FirstName")),
                                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetString(reader.GetOrdinal("PasswordHash")) ?? null

                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions (e.g., logging)
                        throw new ApplicationException("Error fetching user data.", ex);
                    }
                }
            }

            return user;
        }
    }
}
