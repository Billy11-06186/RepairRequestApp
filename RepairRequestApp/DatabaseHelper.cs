using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace RepairRequestApp
{
    public class DatabaseHelper
    {
        private static string databaseFile = "RepairRequests.db";
        private static string connectionString = $"Data Source={databaseFile};Version=3;";
        public static void InitializeDatabase()
        {
            if (!File.Exists(databaseFile))
            {
                SQLiteConnection.CreateFile(databaseFile);
                CreateTables();
                AddTestData();
            }
        }

        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS RepairRequests (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Equipment TEXT NOT NULL,
                        FaultType TEXT NOT NULL,
                        Status TEXT NOT NULL,
                        Client TEXT NOT NULL,
                        Description TEXT,
                        CreatedDate TEXT NOT NULL
                    )";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void AddTestData()
        {
            var testRequests = new List<RepairRequest>
            {
                new RepairRequest { Equipment = "Ноутбук Dell", FaultType = "Не включается",
                    Status = "Новая", Client = "Иванов И.И.", Description = "Ноутбук не реагирует на кнопку включения",
                    CreatedDate = DateTime.Now.AddDays(-5) },
                new RepairRequest { Equipment = "Смартфон iPhone", FaultType = "Разбит экран",
                    Status = "В работе", Client = "Петров П.П.", Description = "Трещины на экране, требуется замена",
                    CreatedDate = DateTime.Now.AddDays(-3) },
                new RepairRequest { Equipment = "Холодильник Samsung", FaultType = "Не морозит",
                    Status = "Завершена", Client = "Сидорова А.А.", Description = "Холодильник работает, но не охлаждает",
                    CreatedDate = DateTime.Now.AddDays(-7) }
            };

            foreach (var request in testRequests)
            {
                AddRepairRequest(request);
            }
        }

        public static List<RepairRequest> GetAllRepairRequests()
        {
            var requests = new List<RepairRequest>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM RepairRequests ORDER BY Id";

                using (var command = new SQLiteCommand(query, connection))
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
                            CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
                        };
                        requests.Add(request);
                    }
                }
            }

            return requests;
        }

        public static void AddRepairRequest(RepairRequest request)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO RepairRequests 
                                (Equipment, FaultType, Status, Client, Description, CreatedDate) 
                                VALUES (@Equipment, @FaultType, @Status, @Client, @Description, @CreatedDate);
                                SELECT last_insert_rowid();";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Equipment", request.Equipment);
                    command.Parameters.AddWithValue("@FaultType", request.FaultType);
                    command.Parameters.AddWithValue("@Status", request.Status);
                    command.Parameters.AddWithValue("@Client", request.Client);
                    command.Parameters.AddWithValue("@Description", request.Description);
                    command.Parameters.AddWithValue("@CreatedDate", request.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                    request.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void UpdateRepairRequest(RepairRequest request)
        {
            using (var connection = new SQLiteConnection(connectionString))
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

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", request.Id);
                    command.Parameters.AddWithValue("@Equipment", request.Equipment);
                    command.Parameters.AddWithValue("@FaultType", request.FaultType);
                    command.Parameters.AddWithValue("@Status", request.Status);
                    command.Parameters.AddWithValue("@Client", request.Client);
                    command.Parameters.AddWithValue("@Description", request.Description);
                    command.Parameters.AddWithValue("@CreatedDate", request.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteRepairRequest(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM RepairRequests WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<RepairRequest> SearchRepairRequests(string searchText)
        {
            var requests = new List<RepairRequest>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM RepairRequests WHERE 
                                Equipment LIKE @Search OR 
                                FaultType LIKE @Search OR 
                                Client LIKE @Search OR 
                                Description LIKE @Search
                                ORDER BY Id";

                using (var command = new SQLiteCommand(query, connection))
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
                                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
                            };
                            requests.Add(request);
                        }
                    }
                }
            }

            return requests;
        }

        public static List<RepairRequest> FilterByStatus(string status)
        {
            var requests = new List<RepairRequest>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM RepairRequests WHERE Status = @Status ORDER BY Id";

                using (var command = new SQLiteCommand(query, connection))
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
                                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
                            };
                            requests.Add(request);
                        }
                    }
                }
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
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM RepairRequests ORDER BY {orderBy}";

                using (var command = new SQLiteCommand(query, connection))
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
                            CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
                        };
                        requests.Add(request);
                    }
                }
            }

            return requests;
        }
    }
}