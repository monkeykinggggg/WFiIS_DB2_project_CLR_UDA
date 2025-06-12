# WFiIS_DB2_project_CLR_UDA

## Requirements
1. Visual Studio 2017+ with .NET Framework 4.8
2. Properly configured appsettings.json with your SQL Server connection string
3. Microsoft SQL Server with a prepared test database


## How to Run

1. **Build the CLR UDA Project**  
   Build the `UDAs` project to generate a `.dll` file containing the custom User-Defined Aggregates written in C#.

2. **Deploy SQL Scripts**  
   Run all scripts from the `SQL/` directory in your SQL Server to:
   - Create necessary tables and procedures
   - Register the CLR UDA assembly

    **Important:** Update the path to your `.dll` in the script before executing it, to reflect your local build path.

3. **Configure the Console Application**  
   Open `Console_App/appsettings.json` and set your SQL Server connection string.

4. **Run the Console App**  
   Run the `Program.cs` file in the `Console_App` project. It will connect to your database and use the installed UDAs.

## Notes
**Unit tests for each UDA are included in the solution under the UDA_Tests project.**
Implemented aggregates:
- Median
- Mode
- Quantile
- Standard Deviation
- Range

For more info see the `Documentation/BD2_Projekt.pdf` file, which contains detailed project documentation with pictures.
