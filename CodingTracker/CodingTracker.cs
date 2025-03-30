using static CodingTracker.Database;
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