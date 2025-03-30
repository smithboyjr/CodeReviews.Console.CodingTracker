using System;
using System.Configuration;
using System.Collections.Specialized;
using Spectre.Console;
using Spectre.Console.Cli;
using static CodingTracker.Database;
using static CodingTracker.HandleDisplay;
using static CodingTracker.ConsoleDisplay;

namespace CodingTracker
{
    public class CodingTracker
    {
        static void Main(string[] args)
        {
            try
            {
                // Initialize the database
                InitializeDatabase();

                // Display the main menu and handle user input
                DisplayMainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}