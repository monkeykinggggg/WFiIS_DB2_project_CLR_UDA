using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Console_App
{
    public class Program
    {
        static void Main()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            Console.WriteLine("Checking DB connection...");
            if (TestDatabaseConnection(connectionString))
            {
                Console.WriteLine("Connection successful!\n");

                HapinessView handler = new HapinessView(connectionString);
                handler.MainMenu();
            }
            else
            {
                Console.WriteLine("Connection failed. Check connection string or SQL Server availability.");
            }
        }

        static bool TestDatabaseConnection(string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during connecting: " + ex.Message);
                return false;
            }
        }

    }
}