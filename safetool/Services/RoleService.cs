using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace safetool.Services
{
    public class RoleService
    {
        private readonly string _connectionString;

        public RoleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<string> GetRolesForUser(string username)
        {
            var roles = new List<string>();

            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                                SELECT r.Name
                                FROM UserRole ur
                                INNER JOIN Role r ON ur.RoleID = r.ID
                                WHERE ur.UserName = @UserName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var roleName = reader["Name"].ToString();
                            roles.Add(roleName);
                        }
                    }
                }
            }
            return roles;
        }
    }
}
