using API;
using ConsoleTables;
using System.Globalization;

namespace Console_App
{
    public class HapinessView
    {
        private HappinessService service;
        public HapinessView(string connectionString)
        {
            service = new HappinessService(connectionString);
        }

        public void WaitForReturnToPreviousMenu()
        {
            Console.WriteLine("Naciśnij dowolny klawisz, aby powrócić do menu...");
            Console.ReadKey();
        }

        public void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("----- Menu Główne -----");
                Console.WriteLine("1. Pokaż tabelkę");
                Console.WriteLine("2. Pokaż statystyki globalne");
                Console.WriteLine("3. Porównaj statystyki dla regionów");
                Console.WriteLine("4. Przeanalizuj statystyki ręcznie");
                Console.WriteLine("q. Wyjście z programu");
                string key = Console.ReadLine().ToLower();

                switch (key)
                {
                    case "1":
                        ShowTable();
                        break;
                    case "2":
                        ShowGlobalStatistics();
                        break;
                    case "3":
                        CompareRegionStatistics();
                        break;
                    case "4":
                        ManualAnalysis();
                        break;
                    case "q":
                        Environment.Exit(0);
                        break;
                }
            }
        }
        private void ShowTable()
        {
            string choice = "";

            while (choice != "r")   // dopoki nie wcisniemy return to jestesmy w petli i robimy zapytania
            {
                Console.Clear();
                Console.WriteLine("Wybierz region: ");
                List<string> regions = service.GetAllRegions();
                int i = 1;
                while (i <= regions.Count)
                {
                    Console.WriteLine(string.Format("{0}. {1}", i, regions[i - 1]));
                    i++;
                }
                Console.WriteLine(string.Format("{0}. Wszystkie regiony", i));
                Console.WriteLine("(r - powrót, q - wyjście z programu)");
                choice = Console.ReadLine().ToLower();

                if (choice == "q")
                {
                    Environment.Exit(0);
                    break;
                }

                bool isNumber = int.TryParse(choice, out int numericChoice);    // parsed number in numericChoice
                ConsoleTable result = null;
                if (isNumber && numericChoice <= regions.Count + 1)
                {
                    if (numericChoice <= regions.Count)
                    {
                        result = service.getTabData(regions[numericChoice - 1]);
                    }
                    else
                    {
                        result = service.getTabData();
                    }
                    result.Write();
                    WaitForReturnToPreviousMenu();

                }

            }
        }

        private void ShowGlobalStatistics()
        {
            string choice = "";
            while (choice != "r")
            {
                Console.Clear();
                Console.WriteLine("1. Ogólne");
                Console.WriteLine("2. Pogrupowane według regionu");
                Console.WriteLine("(r - powrót, q - wyjście z programu)");
                choice = Console.ReadLine().ToLower();

                if (choice == "q")
                {
                    Environment.Exit(0);
                    break;
                }
                bool isNumber = int.TryParse(choice, out int numericChoice);
                if (isNumber)
                {
                    ConsoleTable result = null;
                    if (numericChoice == 1)
                    {
                        result = service.getStatistics();
                    }
                    else if (numericChoice == 2)
                    {
                        result = service.getStatisticsGrouped();
                    }
                    result.Write();
                    WaitForReturnToPreviousMenu();
                }
            }

        }

        private ConsoleTable getStatisticsForRegions(List<string> regionNames)
        {
            ConsoleTable combinedTable = new ConsoleTable("Region", "Ilość krajów", "Mediana", "Moda", "Std", "Przedział", "Kwantyl 0.25", "Kwantyl 0.75", "Min", "Max");

            foreach (string region in regionNames)
            {
                ConsoleTable singleTable = service.getStatistics(region);   // one row table

                if (singleTable.Rows.Count > 0) 
                {
                    var row = singleTable.Rows[0];
                    combinedTable.AddRow(region, row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8]);
                }
            }

            return combinedTable;
        }
        private void CompareRegionStatistics()
        {
            string choice = "";

            while (choice != "r")
            {
                Console.Clear();
                List<string> regions = service.GetAllRegions();

                Console.WriteLine("Liczba regionów do porównania:");
                Console.WriteLine("(r - powrót, q - wyjście z programu)");
                choice = Console.ReadLine().ToLower();

                if (choice == "q")
                {
                    Environment.Exit(0);
                    break;
                }
                if (choice == "r")
                {
                    break;
                }

                bool isNumber = int.TryParse(choice, out int count);
                if (!isNumber || count <= 0 || count > regions.Count)
                {
                    Console.WriteLine("Nieprawidłowa liczba regionów.");
                    WaitForReturnToPreviousMenu();
                    continue;
                }

                List<string> selectedRegions = new List<string>();
                while (selectedRegions.Count < count)
                {
                    Console.Clear();
                    Console.WriteLine("Dostępne regiony:");
                    for (int i = 1; i <= regions.Count; i++)
                    {
                        string selectedMarker = selectedRegions.Contains(regions[i - 1]) ? " -- WYBRANY" : "";
                        Console.WriteLine($"{i}. {regions[i - 1]} {selectedMarker}");
                    }

                    Console.WriteLine($"Wybierz region ({selectedRegions.Count + 1} z {count}):");
                    string input = Console.ReadLine().ToLower();

                    if (input == "q")
                    {
                        Environment.Exit(0);
                        return;
                    }
                    if (input == "r")
                    {
                        break;
                    }

                    if (int.TryParse(input, out int regionIndex) && regionIndex >= 1 && regionIndex <= regions.Count)
                    {
                        string chosenRegion = regions[regionIndex - 1];
                        if (!selectedRegions.Contains(chosenRegion))
                        {
                            selectedRegions.Add(chosenRegion);
                        }
                        else
                        {
                            Console.WriteLine("Ten region już został wybrany.");
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy wybór.");
                        Thread.Sleep(1000);
                    }
                }

                if (selectedRegions.Count == count)
                {
                    ConsoleTable result = getStatisticsForRegions(selectedRegions);
                    result.Write();
                    WaitForReturnToPreviousMenu();
                }
            }
        }

        private void ManualAnalysis()
        {
            string choice = "";

            while (choice != "r")  
            {
                Console.Clear();
                Console.WriteLine("Wybierz region: ");
                List<string> regions = service.GetAllRegions();
                int i = 1;
                while (i <= regions.Count)
                {
                    Console.WriteLine(string.Format("{0}. {1}", i, regions[i - 1]));
                    i++;
                }
                Console.WriteLine("(r - powrót, q - wyjście z programu)");
                choice = Console.ReadLine().ToLower();

                if (choice == "q")
                {
                    Environment.Exit(0);
                    break;
                }

                bool isNumber = int.TryParse(choice, out int numericChoice);    // parsed number in numericChoice
                
                if (isNumber && numericChoice <= regions.Count)
                {
                    string selectedRegion = regions[numericChoice - 1];
                   
                    while(true) {

                        Console.Clear();
                        Console.WriteLine("Wybrany region: " + selectedRegion);
                        Console.WriteLine("Wybierz agregat: ");
                        List<string> aggregates = service.GetAllAggregatesNames();
                        for (int j = 1;j<= aggregates.Count;j++)
                        {
                            Console.WriteLine(string.Format("{0}. {1}", j, aggregates[j - 1]));
                        }
                        Console.WriteLine("(r - powrót, q - wyjście z programu)");
                        string aggChoosen = Console.ReadLine().ToLower();

                        if (aggChoosen == "q") Environment.Exit(0);
                        if (aggChoosen == "r") break;

                        bool isNum = int.TryParse(aggChoosen, out int aggregateIdx);
                        if(isNum && aggregateIdx <= aggregates.Count)
                        {
                            string selectedAggregate = aggregates[aggregateIdx - 1];
                            double? quantile = null;

                            if (selectedAggregate.ToLower() == "quantile")
                            {
                                Console.WriteLine("Podaj wartość kwantyla [0 - 1]:");
                                string quantileInput = Console.ReadLine();
                                if (double.TryParse(quantileInput, NumberStyles.Any, CultureInfo.InvariantCulture, out double qVal) && qVal >= 0 && qVal <= 1)
                                {
                                    quantile = qVal;
                                }
                                else
                                {
                                    Console.WriteLine("Nieprawidłowa wartość kwantyla.");
                                    Thread.Sleep(1000);
                                    continue;
                                }
                            }
                            try
                            {
                                double result = service.GetCustomAggregateForRegion(selectedAggregate, selectedRegion, quantile);
                                Console.WriteLine("Wynik dla " + selectedRegion + " --- " + selectedAggregate + " : " + result);

                                ConsoleTable tabForCheck = service.GetHappinessScoresForRegion(selectedRegion);
                                tabForCheck.Write();
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            WaitForReturnToPreviousMenu();
                        }

                    }


                }

            }



        }


    }
}
