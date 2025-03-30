using Spectre.Console;
using static CodingTracker.HandleDisplay;
using static CodingTracker.Validation;

namespace CodingTracker
{
    public static class ConsoleDisplay
    {
        // Method to display the main menu
        public static void DisplayMainMenu()
        {
            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Coding Tracker!");
                Console.WriteLine("1. Start A New Coding Session");
                Console.WriteLine("2. Delete Sessions");
                Console.WriteLine("3. Update Sessions");
                Console.WriteLine("4. View All Sessions");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");

                // Use GetInput to handle menu input
                string menuOption = GetValidatedInput("Select an option: ", input => int.TryParse(input, out _), "Please enter a valid number.");

                // Delegate the handling of the menu option to HandleMenu
                exitProgram = HandleMenu(menuOption);
            }

            Console.WriteLine("Thank you for using Coding Tracker. Goodbye!");
        }

        // Method to create the sessions table
        private static Table CreateCodingSessionTable()
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");
            return table;
        }

        // Method to display coding sessions in a table format
        public static void DisplayCodingSessions(List<CodingSession> sessions)
        {
            var table = CreateCodingSessionTable();

            if (sessions == null || sessions.Count == 0)
            {
                table.AddRow("[red]No coding sessions found.[/]", "", "", "");
            }
            else
            {
                foreach (var session in sessions)
                {
                    table.AddRow(
                        session.Id.ToString(),
                        session.StartTime.ToString("MM-dd-yyyy HH:mm"),
                        session.EndTime.ToString("MM-dd-yyyy HH:mm"),
                        session.Duration?.ToString() ?? "N/A");
                }
            }

            AnsiConsole.Write(table);
        }
    }
}