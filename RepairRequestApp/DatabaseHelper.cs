using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RepairRequestApp
{
    public class DatabaseHelper
    {
        private static string connectionString = @"Data Source=MSI\MSSQLSERVER02;Initial Catalog=RepairRequestsDB;Integrated Security=True;TrustServerCertificate=True;";

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        public static List<RepairRequest> GetAllRepairRequests()
        {
            var requests = new List<RepairRequest>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM RepairRequests ORDER BY Id";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var request = new RepairRequest
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Equipment = reader["Equipment"].ToString(),
                                FaultType = reader["FaultType"].ToString(),
                                Status = reader["Status"].ToString(),
                                Client = reader["Client"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                            requests.Add(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetAllRepairRequests: {ex.Message}");
                throw;
            }

            return requests;
        }

        public static void AddRepairRequest(RepairRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO RepairRequests 
                                    (Equipment, FaultType, Status, Client, Description, CreatedDate) 
                                    VALUES (@Equipment, @FaultType, @Status, @Client, @Description, @CreatedDate);
                                    SELECT SCOPE_IDENTITY();";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Equipment", request.Equipment);
                        command.Parameters.AddWithValue("@FaultType", request.FaultType);
                        command.Parameters.AddWithValue("@Status", request.Status);
                        command.Parameters.AddWithValue("@Client", request.Client);
                        command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(request.Description) ? "" : request.Description);
                        command.Parameters.AddWithValue("@CreatedDate", request.CreatedDate);

                        request.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка AddRepairRequest: {ex.Message}");
                throw;
            }
        }

        public static void UpdateRepairRequest(RepairRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE RepairRequests SET 
                                    Equipment = @Equipment, 
                                    FaultType = @FaultType, 
                                    Status = @Status, 
                                    Client = @Client, 
                                    Description = @Description,
                                    CreatedDate = @CreatedDate
                                    WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", request.Id);
                        command.Parameters.AddWithValue("@Equipment", request.Equipment);
                        command.Parameters.AddWithValue("@FaultType", request.FaultType);
                        command.Parameters.AddWithValue("@Status", request.Status);
                        command.Parameters.AddWithValue("@Client", request.Client);
                        command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(request.Description) ? "" : request.Description);
                        command.Parameters.AddWithValue("@CreatedDate", request.CreatedDate);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка UpdateRepairRequest: {ex.Message}");
                throw;
            }
        }

        public static void DeleteRepairRequest(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM RepairRequests WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка DeleteRepairRequest: {ex.Message}");
                throw;
            }
        }

        public static List<RepairRequest> SearchRepairRequests(string searchText)
        {
            var requests = new List<RepairRequest>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT * FROM RepairRequests WHERE 
                                    Equipment LIKE @Search OR 
                                    FaultType LIKE @Search OR 
                                    Client LIKE @Search OR 
                                    Description LIKE @Search
                                    ORDER BY Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Search", $"%{searchText}%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var request = new RepairRequest
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Equipment = reader["Equipment"].ToString(),
                                    FaultType = reader["FaultType"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    Client = reader["Client"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                                };
                                requests.Add(request);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка SearchRepairRequests: {ex.Message}");
                throw;
            }

            return requests;
        }

        public static List<RepairRequest> FilterByStatus(string status)
        {
            var requests = new List<RepairRequest>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM RepairRequests WHERE Status = @Status ORDER BY Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var request = new RepairRequest
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Equipment = reader["Equipment"].ToString(),
                                    FaultType = reader["FaultType"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    Client = reader["Client"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                                };
                                requests.Add(request);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка FilterByStatus: {ex.Message}");
                throw;
            }

            return requests;
        }

        public static List<RepairRequest> GetSortedRequests(string sortBy)
        {
            var requests = new List<RepairRequest>();
            string orderBy = "Id";

            switch (sortBy)
            {
                case "По ID":
                    orderBy = "Id";
                    break;
                case "По дате (возр.)":
                    orderBy = "CreatedDate ASC";
                    break;
                case "По дате (убыв.)":
                    orderBy = "CreatedDate DESC";
                    break;
                case "По оборудованию":
                    orderBy = "Equipment";
                    break;
                case "По клиенту":
                    orderBy = "Client";
                    break;
                default:
                    orderBy = "Id";
                    break;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM RepairRequests ORDER BY {orderBy}";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var request = new RepairRequest
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Equipment = reader["Equipment"].ToString(),
                                FaultType = reader["FaultType"].ToString(),
                                Status = reader["Status"].ToString(),
                                Client = reader["Client"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                            requests.Add(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetSortedRequests: {ex.Message}");
                throw;
            }

            return requests;
        }

        public static int GetRequestsCount()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM RepairRequests";

                    using (var command = new SqlCommand(query, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetRequestsCount: {ex.Message}");
                return 0;
            }
        }

        public static int GetRequestsCountByStatus(string status)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM RepairRequests WHERE Status = @Status";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetRequestsCountByStatus: {ex.Message}");
                return 0;
            }
        }
    }
}