using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace AnimalShelter
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["AnimalShelterDB"].ConnectionString;

        // авторизация
        public static string Authenticate(string login, string password)
        {
            string role = null;

            string query = @"
                SELECT r.RoleName
                FROM Users_kursovoi u
                JOIN Role_kursovoi r ON u.RoleID = r.RoleID
                WHERE u.Login = @Login AND u.Password = @Password";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                    role = result.ToString();
            }
            return role;
        }

        // животные
        public static List<Animal> GetAllAnimals()
        {
            var list = new List<Animal>();

            string query = @"
                    SELECT a.AnimalID, a.Nickname, a.SpeciesID, s.SpeciesName,
                           a.BreedID, b.BreedName, a.Age, a.GenderID, g.GenderName,
                           a.HealthStatus, a.AdmissionDate, a.IsAdopted, a.PhotoPath
                    FROM Animals_kursovoi a
                    JOIN Species_kursovoi s ON a.SpeciesID = s.SpeciesID
                    JOIN Breeds_kursovoi b ON a.BreedID = b.BreedID
                    JOIN Gender_kursovoi g ON a.GenderID = g.GenderID
                    ORDER BY a.AnimalID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Animal
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesID = (int)reader["SpeciesID"],
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedID = (int)reader["BreedID"],
                            BreedName = reader["BreedName"].ToString(),
                            Age = reader["Age"] == DBNull.Value ? (int?)null : (int)reader["Age"],
                            GenderID = (int)reader["GenderID"],
                            GenderName = reader["GenderName"].ToString(),
                            HealthStatus = reader["HealthStatus"] == DBNull.Value ? "" : reader["HealthStatus"].ToString(),
                            AdmissionDate = reader["AdmissionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["AdmissionDate"],
                            IsAdopted = (bool)reader["IsAdopted"],
                            PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public static void AddAnimal(Animal a)
        {
            string query = @"
                    INSERT INTO Animals_kursovoi 
                        (Nickname, SpeciesID, BreedID, Age, GenderID, HealthStatus, AdmissionDate, IsAdopted, PhotoPath)
                    VALUES 
                        (@Nickname, @SpeciesID, @BreedID, @Age, @GenderID, @HealthStatus, @AdmissionDate, @IsAdopted, @PhotoPath)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Nickname", a.Nickname);
                cmd.Parameters.AddWithValue("@SpeciesID", a.SpeciesID);
                cmd.Parameters.AddWithValue("@BreedID", a.BreedID);
                cmd.Parameters.AddWithValue("@Age", (object)a.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GenderID", a.GenderID);
                cmd.Parameters.AddWithValue("@HealthStatus", (object)a.HealthStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionDate", (object)a.AdmissionDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAdopted", a.IsAdopted);
                cmd.Parameters.AddWithValue("@PhotoPath", (object)a.PhotoPath ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateAnimal(Animal a)
        {
            string query = @"
                    UPDATE Animals_kursovoi SET
                        Nickname = @Nickname,
                        SpeciesID = @SpeciesID,
                        BreedID = @BreedID,
                        Age = @Age,
                        GenderID = @GenderID,
                        HealthStatus = @HealthStatus,
                        AdmissionDate = @AdmissionDate,
                        IsAdopted = @IsAdopted,
                        PhotoPath = @PhotoPath
                    WHERE AnimalID = @AnimalID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@AnimalID", a.AnimalID);
                cmd.Parameters.AddWithValue("@Nickname", a.Nickname);
                cmd.Parameters.AddWithValue("@SpeciesID", a.SpeciesID);
                cmd.Parameters.AddWithValue("@BreedID", a.BreedID);
                cmd.Parameters.AddWithValue("@Age", (object)a.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GenderID", a.GenderID);
                cmd.Parameters.AddWithValue("@HealthStatus", (object)a.HealthStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionDate", (object)a.AdmissionDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAdopted", a.IsAdopted);
                cmd.Parameters.AddWithValue("@PhotoPath", (object)a.PhotoPath ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteAnimal(int animalId)
        {
            string query = "DELETE FROM Animals_kursovoi WHERE AnimalID = @AnimalID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@AnimalID", animalId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // справочники (для выпадающих списков)
        public static List<Tuple<int, string>> GetSpeciesList()
        {
            return GetSimpleList("Species_kursovoi", "SpeciesID", "SpeciesName");
        }

        public static List<Tuple<int, string>> GetGendersList()
        {
            return GetSimpleList("Gender_kursovoi", "GenderID", "GenderName");
        }

        public static List<Tuple<int, string>> GetBreedsBySpecies(int speciesId)
        {
            var list = new List<Tuple<int, string>>();
            string query = "SELECT BreedID, BreedName FROM Breeds_kursovoi WHERE SpeciesID = @SpeciesID ORDER BY BreedName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SpeciesID", speciesId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create((int)reader[0], reader[1].ToString()));
                    }
                }
            }
            return list;
        }

        private static List<Tuple<int, string>> GetSimpleList(string table, string idCol, string nameCol)
        {
            var list = new List<Tuple<int, string>>();
            string query = $"SELECT {idCol}, {nameCol} FROM {table} ORDER BY {nameCol}";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create((int)reader[0], reader[1].ToString()));
                    }
                }
            }
            return list;
        }

        // посетители
        public static List<Visitor> GetAllVisitors()
        {
            var list = new List<Visitor>();

            string query = @"
                SELECT v.VisitorID, v.LastName, v.FirstName, v.Patronymic,
                       v.Phone, v.PurposeID, p.PurposeName
                FROM Visitors_kursovoi v
                JOIN VisitPurposes_kursovoi p ON v.PurposeID = p.PurposeID
                ORDER BY v.VisitorID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Visitor
                        {
                            VisitorID = (int)reader["VisitorID"],
                            LastName = reader["LastName"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            Patronymic = reader["Patronymic"] == DBNull.Value ? "" : reader["Patronymic"].ToString(),
                            Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                            PurposeID = (int)reader["PurposeID"],
                            PurposeName = reader["PurposeName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public static void AddVisitor(Visitor v)
        {
            string query = @"
                INSERT INTO Visitors_kursovoi (LastName, FirstName, Patronymic, Phone, PurposeID)
                VALUES (@LastName, @FirstName, @Patronymic, @Phone, @PurposeID)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LastName", v.LastName);
                cmd.Parameters.AddWithValue("@FirstName", v.FirstName);
                cmd.Parameters.AddWithValue("@Patronymic", (object)v.Patronymic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)v.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PurposeID", v.PurposeID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateVisitor(Visitor v)
        {
            string query = @"
                UPDATE Visitors_kursovoi SET
                    LastName = @LastName,
                    FirstName = @FirstName,
                    Patronymic = @Patronymic,
                    Phone = @Phone,
                    PurposeID = @PurposeID
                WHERE VisitorID = @VisitorID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VisitorID", v.VisitorID);
                cmd.Parameters.AddWithValue("@LastName", v.LastName);
                cmd.Parameters.AddWithValue("@FirstName", v.FirstName);
                cmd.Parameters.AddWithValue("@Patronymic", (object)v.Patronymic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)v.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PurposeID", v.PurposeID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteVisitor(int visitorId)
        {
            string query = "DELETE FROM Visitors_kursovoi WHERE VisitorID = @VisitorID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VisitorID", visitorId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Tuple<int, string>> GetPurposesList()
        {
            return GetSimpleList("VisitPurposes_kursovoi", "PurposeID", "PurposeName");
        }

        // сотрудники
        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();

            string query = @"
                SELECT EmployeeID, LastName, FirstName, Patronymic, Position, Phone
                FROM Employees_kursovoi
                ORDER BY EmployeeID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Employee
                        {
                            EmployeeID = (int)reader["EmployeeID"],
                            LastName = reader["LastName"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            Patronymic = reader["Patronymic"] == DBNull.Value ? "" : reader["Patronymic"].ToString(),
                            Position = reader["Position"] == DBNull.Value ? "" : reader["Position"].ToString(),
                            Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public static void AddEmployee(Employee emp)
        {
            string query = @"
                INSERT INTO Employees_kursovoi (LastName, FirstName, Patronymic, Position, Phone)
                VALUES (@LastName, @FirstName, @Patronymic, @Position, @Phone)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                cmd.Parameters.AddWithValue("@Patronymic", (object)emp.Patronymic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Position", (object)emp.Position ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)emp.Phone ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateEmployee(Employee emp)
        {
            string query = @"
                UPDATE Employees_kursovoi SET
                    LastName = @LastName,
                    FirstName = @FirstName,
                    Patronymic = @Patronymic,
                    Position = @Position,
                    Phone = @Phone
                WHERE EmployeeID = @EmployeeID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                cmd.Parameters.AddWithValue("@Patronymic", (object)emp.Patronymic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Position", (object)emp.Position ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)emp.Phone ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteEmployee(int employeeId)
        {
            string query = "DELETE FROM Employees_kursovoi WHERE EmployeeID = @EmployeeID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // усыновления
        public static List<Adoption> GetAllAdoptions()
        {
            var list = new List<Adoption>();

            string query = @"
                SELECT a.AdoptionID, 
                       a.AnimalID, an.Nickname AS AnimalNickname,
                       a.VisitorID, v.LastName + ' ' + v.FirstName AS VisitorName,
                       v.Phone AS VisitorPhone,
                       a.EmployeeID, e.LastName + ' ' + e.FirstName AS EmployeeName,
                       a.AdoptionDate, a.Notes, a.NextContactDate
                FROM Adoptions_kursovoi a
                JOIN Animals_kursovoi an ON a.AnimalID = an.AnimalID
                JOIN Visitors_kursovoi v ON a.VisitorID = v.VisitorID
                JOIN Employees_kursovoi e ON a.EmployeeID = e.EmployeeID
                ORDER BY a.AdoptionID DESC";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Adoption
                        {
                            AdoptionID = (int)reader["AdoptionID"],
                            AnimalID = (int)reader["AnimalID"],
                            AnimalNickname = reader["AnimalNickname"].ToString(),
                            VisitorID = (int)reader["VisitorID"],
                            VisitorName = reader["VisitorName"].ToString(),
                            VisitorPhone = reader["VisitorPhone"] == DBNull.Value ? "" : reader["VisitorPhone"].ToString(),
                            EmployeeID = (int)reader["EmployeeID"],
                            EmployeeName = reader["EmployeeName"].ToString(),
                            AdoptionDate = (DateTime)reader["AdoptionDate"],
                            Notes = reader["Notes"] == DBNull.Value ? "" : reader["Notes"].ToString(),
                            NextContactDate = reader["NextContactDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["NextContactDate"]
                        });
                    }
                }
            }

            return list;
        }

        public static int AddAdoption(Adoption a)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string insertQuery = @"
                    INSERT INTO Adoptions_kursovoi (AnimalID, VisitorID, EmployeeID, AdoptionDate, Notes, NextContactDate)
                    VALUES (@AnimalID, @VisitorID, @EmployeeID, @AdoptionDate, @Notes, @NextContactDate);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newId;
                using (var cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", a.AnimalID);
                    cmd.Parameters.AddWithValue("@VisitorID", a.VisitorID);
                    cmd.Parameters.AddWithValue("@EmployeeID", a.EmployeeID);
                    cmd.Parameters.AddWithValue("@AdoptionDate", a.AdoptionDate);
                    cmd.Parameters.AddWithValue("@Notes", (object)a.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NextContactDate", (object)a.NextContactDate ?? DBNull.Value);
                    newId = (int)cmd.ExecuteScalar();
                }

                string updateQuery = "UPDATE Animals_kursovoi SET IsAdopted = 1 WHERE AnimalID = @AnimalID";
                using (var cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", a.AnimalID);
                    cmd.ExecuteNonQuery();
                }

                return newId;
            }
        }

        public static void DeleteAdoption(int adoptionId, int animalId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM Adoptions_kursovoi WHERE AdoptionID = @AdoptionID";
                using (var cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AdoptionID", adoptionId);
                    cmd.ExecuteNonQuery();
                }

                string updateQuery = "UPDATE Animals_kursovoi SET IsAdopted = 0 WHERE AnimalID = @AnimalID";
                using (var cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AnimalID", animalId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Tuple<int, string>> GetAvailableAnimals()
        {
            var list = new List<Tuple<int, string>>();
            string query = @"
                SELECT AnimalID, Nickname 
                FROM Animals_kursovoi 
                WHERE IsAdopted = 0 
                ORDER BY Nickname";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create((int)reader[0], reader[1].ToString()));
                    }
                }
            }
            return list;
        }

        public static List<Tuple<int, string>> GetVisitorsForCombo()
        {
            var list = new List<Tuple<int, string>>();
            string query = @"
                SELECT VisitorID, LastName + ' ' + FirstName + ISNULL(' ' + Patronymic, '') AS FIO
                FROM Visitors_kursovoi 
                ORDER BY LastName, FirstName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create((int)reader[0], reader[1].ToString()));
                    }
                }
            }
            return list;
        }

        public static List<Tuple<int, string>> GetEmployeesForCombo()
        {
            var list = new List<Tuple<int, string>>();
            string query = @"
                SELECT EmployeeID, LastName + ' ' + FirstName + ISNULL(' ' + Patronymic, '') AS FIO
                FROM Employees_kursovoi 
                ORDER BY LastName, FirstName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create((int)reader[0], reader[1].ToString()));
                    }
                }
            }
            return list;
        }

        public static List<AnimalInShelterReport> GetAnimalsInShelterReport()
        {
            var list = new List<AnimalInShelterReport>();

            string query = @"
                SELECT a.AnimalID, a.Nickname, 
                       s.SpeciesName, b.BreedName, 
                       a.Age, g.GenderName, 
                       a.HealthStatus, a.AdmissionDate
                FROM Animals_kursovoi a
                JOIN Species_kursovoi s ON a.SpeciesID = s.SpeciesID
                JOIN Breeds_kursovoi b ON a.BreedID = b.BreedID
                JOIN Gender_kursovoi g ON a.GenderID = g.GenderID
                WHERE a.IsAdopted = 0
                ORDER BY a.AdmissionDate DESC, a.Nickname";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AnimalInShelterReport
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedName = reader["BreedName"].ToString(),
                            Age = reader["Age"] == DBNull.Value ? (int?)null : (int)reader["Age"],
                            GenderName = reader["GenderName"].ToString(),
                            HealthStatus = reader["HealthStatus"] == DBNull.Value ? "" : reader["HealthStatus"].ToString(),
                            AdmissionDate = reader["AdmissionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["AdmissionDate"]
                        });
                    }
                }
            }

            return list;
        }

        public static List<AdoptedAnimalReport> GetAdoptedAnimalsReport()
        {
            var list = new List<AdoptedAnimalReport>();

            string query = @"
                SELECT an.AnimalID, an.Nickname, 
                       s.SpeciesName, b.BreedName,
                       v.LastName + ' ' + v.FirstName AS VisitorName,
                       e.LastName + ' ' + e.FirstName AS EmployeeName,
                       ad.AdoptionDate
                FROM Adoptions_kursovoi ad
                JOIN Animals_kursovoi an ON ad.AnimalID = an.AnimalID
                JOIN Species_kursovoi s ON an.SpeciesID = s.SpeciesID
                JOIN Breeds_kursovoi b ON an.BreedID = b.BreedID
                JOIN Visitors_kursovoi v ON ad.VisitorID = v.VisitorID
                JOIN Employees_kursovoi e ON ad.EmployeeID = e.EmployeeID
                ORDER BY ad.AdoptionDate DESC";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AdoptedAnimalReport
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedName = reader["BreedName"].ToString(),
                            VisitorName = reader["VisitorName"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            AdoptionDate = (DateTime)reader["AdoptionDate"]
                        });
                    }
                }
            }

            return list;
        }

        public static List<SpeciesStatReport> GetSpeciesStatistics()
        {
            var list = new List<SpeciesStatReport>();

            string query = @"
                SELECT s.SpeciesName,
                       COUNT(a.AnimalID) AS TotalCount,
                       SUM(CASE WHEN a.IsAdopted = 0 THEN 1 ELSE 0 END) AS InShelterCount,
                       SUM(CASE WHEN a.IsAdopted = 1 THEN 1 ELSE 0 END) AS AdoptedCount
                FROM Species_kursovoi s
                LEFT JOIN Animals_kursovoi a ON s.SpeciesID = a.SpeciesID
                GROUP BY s.SpeciesName
                ORDER BY s.SpeciesName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SpeciesStatReport
                        {
                            SpeciesName = reader["SpeciesName"].ToString(),
                            TotalCount = Convert.ToInt32(reader["TotalCount"]),
                            InShelterCount = reader["InShelterCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InShelterCount"]),
                            AdoptedCount = reader["AdoptedCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AdoptedCount"])
                        });
                    }
                }
            }

            return list;
        }

        public static List<AdoptionStatReport> GetAdoptionsByPeriod(DateTime from, DateTime to)
        {
            var list = new List<AdoptionStatReport>();

            string query = @"
                SELECT ad.AdoptionID,
                       an.Nickname AS AnimalNickname,
                       v.LastName + ' ' + v.FirstName AS VisitorName,
                       e.LastName + ' ' + e.FirstName AS EmployeeName,
                       ad.AdoptionDate
                FROM Adoptions_kursovoi ad
                JOIN Animals_kursovoi an ON ad.AnimalID = an.AnimalID
                JOIN Visitors_kursovoi v ON ad.VisitorID = v.VisitorID
                JOIN Employees_kursovoi e ON ad.EmployeeID = e.EmployeeID
                WHERE ad.AdoptionDate BETWEEN @From AND @To
                ORDER BY ad.AdoptionDate";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@From", from);
                cmd.Parameters.AddWithValue("@To", to);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AdoptionStatReport
                        {
                            AdoptionID = (int)reader["AdoptionID"],
                            AnimalNickname = reader["AnimalNickname"].ToString(),
                            VisitorName = reader["VisitorName"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            AdoptionDate = (DateTime)reader["AdoptionDate"]
                        });
                    }
                }
            }

            return list;
        }

        // виды
        public static void AddSpecies(string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("INSERT INTO Species_kursovoi (SpeciesName) VALUES (@Name)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateSpecies(int id, string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("UPDATE Species_kursovoi SET SpeciesName = @Name WHERE SpeciesID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteSpecies(int id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("DELETE FROM Species_kursovoi WHERE SpeciesID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // породы
        public static List<Tuple<int, string, int>> GetAllBreeds()
        {
            var list = new List<Tuple<int, string, int>>();
            string query = "SELECT BreedID, BreedName, SpeciesID FROM Breeds_kursovoi ORDER BY BreedName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Tuple.Create(
                            (int)reader["BreedID"],
                            reader["BreedName"].ToString(),
                            (int)reader["SpeciesID"]));
                    }
                }
            }
            return list;
        }

        public static void AddBreed(string name, int speciesId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("INSERT INTO Breeds_kursovoi (BreedName, SpeciesID) VALUES (@Name, @SpeciesID)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@SpeciesID", speciesId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateBreed(int id, string name, int speciesId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("UPDATE Breeds_kursovoi SET BreedName = @Name, SpeciesID = @SpeciesID WHERE BreedID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@SpeciesID", speciesId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteBreed(int id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("DELETE FROM Breeds_kursovoi WHERE BreedID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // полы
        public static void AddGender(string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("INSERT INTO Gender_kursovoi (GenderName) VALUES (@Name)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateGender(int id, string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("UPDATE Gender_kursovoi SET GenderName = @Name WHERE GenderID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteGender(int id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("DELETE FROM Gender_kursovoi WHERE GenderID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // цели визита
        public static void AddPurpose(string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("INSERT INTO VisitPurposes_kursovoi (PurposeName) VALUES (@Name)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdatePurpose(int id, string name)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("UPDATE VisitPurposes_kursovoi SET PurposeName = @Name WHERE PurposeID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeletePurpose(int id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("DELETE FROM VisitPurposes_kursovoi WHERE PurposeID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // смена пароля
        public static bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            string checkQuery = "SELECT COUNT(*) FROM Users_kursovoi WHERE Login = @Login AND Password = @OldPassword";

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@OldPassword", oldPassword);
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                        return false;
                }

                string updateQuery = "UPDATE Users_kursovoi SET Password = @NewPassword WHERE Login = @Login";
                using (var cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static List<AdoptionStatReport> GetUpcomingContacts(int days)
        {
            var list = new List<AdoptionStatReport>();

            string query = @"
                SELECT ad.AdoptionID,
                       an.Nickname AS AnimalNickname,
                       v.LastName + ' ' + v.FirstName AS VisitorName,
                       e.LastName + ' ' + e.FirstName AS EmployeeName,
                       ad.AdoptionDate
                FROM Adoptions_kursovoi ad
                JOIN Animals_kursovoi an ON ad.AnimalID = an.AnimalID
                JOIN Visitors_kursovoi v ON ad.VisitorID = v.VisitorID
                JOIN Employees_kursovoi e ON ad.EmployeeID = e.EmployeeID
                WHERE ad.NextContactDate IS NOT NULL 
                  AND ad.NextContactDate <= DATEADD(day, @Days, CAST(GETDATE() AS DATE))
                ORDER BY ad.NextContactDate";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Days", days);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AdoptionStatReport
                        {
                            AdoptionID = (int)reader["AdoptionID"],
                            AnimalNickname = reader["AnimalNickname"].ToString(),
                            VisitorName = reader["VisitorName"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            AdoptionDate = (DateTime)reader["AdoptionDate"]
                        });
                    }
                }
            }

            return list;
        }

        // счётчики для главного экрана
        public static DashboardCounters GetDashboardCounters()
        {
            var counters = new DashboardCounters();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // животных в приюте (не усыновлены)
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Animals_kursovoi WHERE IsAdopted = 0", conn))
                    counters.AnimalsInShelter = (int)cmd.ExecuteScalar();

                // усыновлено за последние 30 дней
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Adoptions_kursovoi WHERE AdoptionDate >= DATEADD(day, -30, CAST(GETDATE() AS DATE))", conn))
                    counters.AdoptedLastMonth = (int)cmd.ExecuteScalar();

                // всего посетителей
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Visitors_kursovoi", conn))
                    counters.TotalVisitors = (int)cmd.ExecuteScalar();

                // всего сотрудников
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Employees_kursovoi", conn))
                    counters.TotalEmployees = (int)cmd.ExecuteScalar();
            }

            return counters;
        }

        // животные, которые давно в приюте (по дате поступления)
        public static List<Animal> GetOldestAnimals(int count)
        {
            var list = new List<Animal>();

            string query = $@"
                SELECT TOP {count} a.AnimalID, a.Nickname, a.SpeciesID, s.SpeciesName,
                       a.BreedID, b.BreedName, a.Age, a.GenderID, g.GenderName,
                       a.HealthStatus, a.AdmissionDate, a.IsAdopted, a.PhotoPath
                FROM Animals_kursovoi a
                JOIN Species_kursovoi s ON a.SpeciesID = s.SpeciesID
                JOIN Breeds_kursovoi b ON a.BreedID = b.BreedID
                JOIN Gender_kursovoi g ON a.GenderID = g.GenderID
                WHERE a.IsAdopted = 0
                ORDER BY a.AdmissionDate ASC";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Animal
                        {
                            AnimalID = (int)reader["AnimalID"],
                            Nickname = reader["Nickname"].ToString(),
                            SpeciesID = (int)reader["SpeciesID"],
                            SpeciesName = reader["SpeciesName"].ToString(),
                            BreedID = (int)reader["BreedID"],
                            BreedName = reader["BreedName"].ToString(),
                            Age = reader["Age"] == DBNull.Value ? (int?)null : (int)reader["Age"],
                            GenderID = (int)reader["GenderID"],
                            GenderName = reader["GenderName"].ToString(),
                            HealthStatus = reader["HealthStatus"] == DBNull.Value ? "" : reader["HealthStatus"].ToString(),
                            AdmissionDate = reader["AdmissionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["AdmissionDate"],
                            IsAdopted = (bool)reader["IsAdopted"],
                            PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}