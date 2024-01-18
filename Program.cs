// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;

namespace ConsoleApp1;

class Program
{
    static string connectionString = "";
    private static IEnumerable<object> allMountains;

    static void Main()

    {
        connectionString = GetConnectionString();
        var repository = new MountainRepository(connectionString);
        // repository.AddMountain("Everest", 8848.86, "Nepal");
        // var mountainsToAdd = new List<Mountain>
        // {
        //    new Mountain { Name = "Dhaulagiri", Height = 8167, Country = "Nepal" };
        //     new Mountain { Name = "Aconcagua", Height = 6961, Country = "Argentina" };
        // };
        // repository.AddMountains(mountainsToAdd);
        //  var allMountains = repository.GetAllMountains();//
        // Console.WriteLine("All Mountains:");
        // foreach (var mountain in allMountains)
        // {
        //     Console.WriteLine($"{mountain.Name} - {mountain.Height} meters in {mountain.Country}");
        // }
        //var specificMountain = repository.GetMountainById(1);
        //if (specificMountain != null)
        //{
        //Console.WriteLine($"Mountain with Id {specificMountain.Id}: {specificMountain.Name}");
        //}
        //repository.DeleteHighestMountain();
        //repository.DeleteHighestMountainByCountry("Nepal");
    }

    static string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"ConnectionString: {connectionString}");

        return connectionString;
    }

    public class MountainRepository
    {
        private string connectionString;

        public MountainRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Метод для добавления одной горы
        public void AddMountain(string name, double height, string country)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Mountains (Name, Height, Country) VALUES (@Name, @Height, @Country)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Height", height);
                    command.Parameters.AddWithValue("@Country", country);

                    command.ExecuteNonQuery();
                }
            }
        }


        // Метод для добавления переменного количества гор
        public void AddMountains(List<Mountain> mountains)
        {
            foreach (var mountain in mountains)
            {
                AddMountain(mountain.Name, mountain.Height, mountain.Country);
            }
        }

        // Метод для получения всех гор
        public List<Mountain> GetAllMountains()
        {
            List<Mountain> mountains = new List<Mountain>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Mountains";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mountains.Add(new Mountain
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Height = Convert.ToDouble(reader["Height"]),
                                Country = (string)reader["Country"]
                            });
                        }
                    }
                }
            }

            return mountains;
        }

        // Метод для получения горы по Id
        public Mountain GetMountainById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Mountains WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Mountain
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Height = Convert.ToDouble(reader["Height"]),
                                Country = (string)reader["Country"]
                            };
                        }
                    }
                }
            }

            return null;
        }

        // Метод для удаления самой высокой горы
        public void DeleteHighestMountain()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Mountains WHERE Height = (SELECT MAX(Height) FROM Mountains)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для удаления самой высокой горы заданной страны
        public void DeleteHighestMountainByCountry(string country)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Mountains WHERE Country = @Country AND Height = (SELECT MAX(Height) FROM Mountains WHERE Country = @Country)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Country", country);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class Mountain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public string Country { get; set; }
    }
 }

   