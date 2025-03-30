using static CodingTracker.ConsoleDisplay;
using static CodingTracker.Validation;
using static CodingTracker.Database;
using static CodingTracker.LogHandler;

namespace CodingTracker
{
    public class HandleDisplay
    {
        // Method handle menu functions
        public static bool HandleMenu(string menuOption)
        {
            try
            {
                switch (menuOption)
                {
                    case "1":
                        // Start a new coding session
                        StartNewSession();
                        break;

                    case "2":
                        // Delete a coding session
                        DeleteSession();
                        break;

                    case "3":
                        // Update an existing coding session
                        UpdateSession();
                        break;

                    case "4":
                        // View all coding sessions
                        ViewAllSessions();
                        break;

                    case "5":
                        // Exit the application
                        return ExitApplication();

                    default:
                        // This case should never be reached due to validation
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadLine();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred. Returning to the main menu.");
                Log($"[Unexpected Error] {ex.Message}");
            }

            return false; // Continue the application
        }

        // Method to start a new coding session
        private static void StartNewSession()
        {
            Console.Clear();
            Console.WriteLine("Starting a new coding session...");

            try
            {
                //Get start and end times from the user
                DateTime startTime = GetValidatedDateTime("Enter the start time (MM-dd-yyyy HH:mm): ");
                DateTime endTime = GetValidatedDateTime("Enter the end time (MM-dd-yyyy HH:mm): ");

                //Validate that the end time is after the start time
                if (endTime <= startTime)
                {
                    Console.WriteLine("End time must be after the start time. Please try again.");
                    return;
                }

                //Create a new CodingSession object
                var session = new CodingSession
                {
                    StartTime = startTime,
                    EndTime = endTime
                };

                //Use the existing method to calculate the duration
                session.CalculateDuration();

                //Save the session to the database
                InsertSessionToDatabase(session);

                //Provide duration to the user
                Console.WriteLine($"Session logged successfully! Duration: {session.Duration}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while starting a new session: {ex.Message}");
                Log($"[Error] StartNewSession: {ex.Message}");
            }

            Console.ReadLine(); // Pause to let the user see the message
        }

        // Method to delete a coding session
        private static void DeleteSession()
        {
            Console.Clear();

            try
            {
                // Retrieve all sessions
                var sessions = GetAllSessions();

                // Check if there are any sessions to delete
                if (sessions.Count == 0)
                {
                    Console.WriteLine("No coding sessions found to delete.");
                    Console.ReadLine();
                    return;
                }

                // Display all sessions
                DisplayCodingSessions(sessions);

                // Prompt the user for the session ID to delete
                int sessionId = GetValidatedID(
                    "Enter the ID of the session you want to delete: ",
                    input => sessions.Any(s => s.Id == input),
                    "Invalid ID. Please enter a valid session ID from the list."
                );

                // Delete the session from the database
                DeleteSessionFromDatabase(sessionId);

                // Provide feedback to the user
                Console.WriteLine($"Session with ID {sessionId} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the session: {ex.Message}");
                Log($"[Error] DeleteSession: {ex.Message}");
            }

            Console.ReadLine(); // Pause to let the user see the message
        }

        // Method to update an existing coding session
        private static void UpdateSession()
        {
            Console.WriteLine("Updating coding sessions...");
            Console.WriteLine("Feature under development.");
            Console.ReadLine();
        }

        // Method to view all coding sessions
        private static void ViewAllSessions()
        {
            Console.Clear();
            Console.WriteLine("Viewing all coding sessions...");
            var sessions = GetAllSessions();

            if (sessions.Count == 0)
            {
                Console.WriteLine("No coding sessions found.");
            }
            else
            {
                DisplayCodingSessions(sessions);
            }

            Console.ReadLine();
        }

        // Method to exit the application
        private static bool ExitApplication()
        {
            Console.WriteLine("Are you sure you want to exit?");
            string input = GetValidatedInput(
                    "Please enter your choice (y/n):",
                    input => input == "y" || input == "n",
                    "Invalid input. Please enter 'y' for yes or 'n' for no."
                    ).ToLower();

            return input == "y";
        }

    }
}