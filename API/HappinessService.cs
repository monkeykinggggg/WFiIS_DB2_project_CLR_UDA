using ConsoleTables;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace API
{
    
    public class HappinessService
    {
        private readonly SqlConnection connection = null;

        public HappinessService(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public List<string> GetAllRegions()
        {
            SqlCommand command = new SqlCommand("usp_GetRegions", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using(SqlDataReader result = command.ExecuteReader())
            {
                try
                {
                    List<string> regionsList = new List<string>();
                    while (result.Read()) {
                        regionsList.Add((string)result["Region"]);
                    }
                    return regionsList;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

        }

        public List<string> GetAllAggregatesNames()
        {
            SqlCommand command = new SqlCommand("usp_GetAggregatesNames", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using (SqlDataReader result = command.ExecuteReader())
            {
                try
                {
                    List<string> aggregatesNames = new List<string>();
                    while (result.Read())
                    {
                        aggregatesNames.Add((string)result["AggregateFunctionName"]);
                    }
                    return aggregatesNames;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

        }
        public ConsoleTable getTabData(string regionName = null)
        {
            string query = @"SELECT ID, CountryName, Region, Score FROM HappinessTab";
            if (!string.IsNullOrEmpty(regionName))
            {
                query += @" WHERE Region = @Region";
            }
            query += @" ORDER BY Score DESC;";

            using(SqlCommand cmd = new SqlCommand(query, this.connection))
            {
                if (!string.IsNullOrEmpty(regionName))
                {
                    cmd.Parameters.AddWithValue("@Region", regionName);
                }
                using (SqlDataReader result = cmd.ExecuteReader()) 
                {
                    ConsoleTable table = new ConsoleTable("ID", "Kraj", "Region", "Wskaźnik Szczęścia");
                    while (result.Read())
                    {
                        table.AddRow(result[0], result[1], result[2], result[3]);
                    }
                    return table;

                }
            }
            
            
        }

        public ConsoleTable getStatistics(string name=null)
        {
            string query = @"
            SELECT 
                COUNT(*) AS CountryCount,
                dbo.Median(Score) AS MedianScore,
                dbo.Mode(Score) AS ModeScore,
                dbo.StdDev(Score) AS StdDev,
                dbo.Range(Score) AS ScoreRange,
                dbo.Quantile(Score,0.25) as Quantile25,
                dbo.Quantile(Score,0.75) as Quantile75,
                MIN(Score) AS MinScore,
                MAX(Score) AS MaxScore
            FROM dbo.HappinessTab";

            if (!string.IsNullOrEmpty(name))
            {
                query += @" WHERE Region = @Region";
            }

            using (SqlCommand cmd = new SqlCommand(query, this.connection))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    cmd.Parameters.AddWithValue("@Region", name);
                }
                using (SqlDataReader result = cmd.ExecuteReader())
                {
                    ConsoleTable table = new ConsoleTable("Ilość krajów", "Mediana", "Moda", "Std", "Przedzial", "Kwantyl 0.25", "Kwantyl 0.75", "Min", "Max");
                    while (result.Read())
                    {
                        int count = result.GetInt32(0);
                        double median = result.GetDouble(1);
                        double mode = result.GetDouble(2);
                        double stdDev = Math.Round(result.GetDouble(3),3);
                        double range = Math.Round(result.GetDouble(4),3);
                        double q25 = result.GetDouble(5);
                        double q75 = result.GetDouble(6);
                        double min = result.GetDouble(7);
                        double max = result.GetDouble(8);

                        table.AddRow(count, median, mode, stdDev, range, q25, q75, min, max);
                    }
                    return table;

                }
            }
        }

        public ConsoleTable getStatisticsGrouped()
        {
            string query = @"
            SELECT Region,
            COUNT(*) AS CountryCount,
            dbo.Median(Score) AS MedianScore,
            dbo.Mode(Score) AS ModeScore,
            dbo.StdDev(Score) AS StdDev,
            dbo.Range(Score) AS ScoreRange,
            dbo.Quantile(Score, 0.25) AS Quantile25,
            dbo.Quantile(Score, 0.75) AS Quantile75,
            MIN(Score) AS MinScore,
            MAX(Score) AS MaxScore
            FROM dbo.HappinessTab 
            GROUP BY Region";

            using (SqlCommand cmd = new SqlCommand(query, this.connection))
            {
                using (SqlDataReader result = cmd.ExecuteReader())
                {
                    ConsoleTable table = new ConsoleTable("Region", "Ilość krajów", "Mediana", "Moda", "Std", "Przedział", "Kwantyl 0.25", "Kwantyl 0.75", "Min", "Max");
                    while (result.Read())
                    {
                        string region = result.GetString(0);
                        int count = result.GetInt32(1);
                        double median = result.GetDouble(2);
                        double mode = result.GetDouble(3);
                        double stdDev = Math.Round(result.GetDouble(4), 3);
                        double range = Math.Round(result.GetDouble(5), 3);
                        double q25 = result.GetDouble(6);
                        double q75 = result.GetDouble(7);
                        double min = result.GetDouble(8);
                        double max = result.GetDouble(9);

                        table.AddRow(region, count, median, mode, stdDev, range, q25, q75, min, max);
                    }
                    return table;

                }
            }


        }

        public ConsoleTable GetHappinessScoresForRegion(string region)
        {
            string query = @"
            SELECT CountryName, Score 
            FROM dbo.HappinessTab 
            WHERE Region = @region
            ORDER BY Score ASC"; 

            using (SqlCommand cmd = new SqlCommand(query, this.connection))
            {
                cmd.Parameters.AddWithValue("@region", region);

                
                using (SqlDataReader result = cmd.ExecuteReader())
                {
                    ConsoleTable table = new ConsoleTable("Kraj", "Wskaźnik Szczęścia");
                    while (result.Read())
                    {
                        string country = result.GetString(0);
                        double score = result.GetDouble(1);

                        table.AddRow(country, score);
                    }
                    return table;

                }
            }
        }

        public double GetCustomAggregateForRegion(string selectedAggregate, string selectedRegion, double? quantile = null)
        {
            bool isQuantile = selectedAggregate.ToLower() == "quantile";
            string query = isQuantile ?
                @"
                SELECT dbo.Quantile(Score, @quantileNumber)
                FROM dbo.HappinessTab 
                WHERE Region = @region
                " : $@"
                    SELECT dbo.{selectedAggregate}(Score)
                    FROM dbo.HappinessTab 
                    WHERE Region = @region";
            using(SqlCommand cmd = new SqlCommand(query, this.connection))
            {
                cmd.Parameters.AddWithValue("@region", selectedRegion);
                if (isQuantile && quantile != null)
                {
                    cmd.Parameters.AddWithValue("@quantileNumber", quantile);
                }

                object result = cmd.ExecuteScalar();
                if(result == null)
                {
                    throw new NullReferenceException("Getinng null value");
                }
                return Convert.ToDouble(result);
            }
   
        }

    }
}
