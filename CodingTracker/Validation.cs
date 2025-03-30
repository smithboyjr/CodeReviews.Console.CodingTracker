using static CodingTracker.LogHandler;

namespace CodingTracker
{
    public static class Validation
    {
        // Method to validate a single CodingSession
        public static bool ValidateSession(CodingSession session, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (session == null)
            {
                errorMessage = "Session is null.";
                return false;
            }

            if (session.StartTime == default || session.EndTime == default)
            {
                errorMessage = $"Session with ID {session.Id} has invalid StartTime or EndTime.";
                return false;
            }

            if (session.EndTime <= session.StartTime)
            {
                errorMessage = $"Session with ID {session.Id} has EndTime before or equal to StartTime.";
                return false;
            }

            // Validate Duration
            var calculatedDuration = session.EndTime - session.StartTime;
            if (session.Duration != calculatedDuration.ToString())
            {
                errorMessage = $"Session with ID {session.Id} has an inconsistent Duration.";
                return false;
            }

            return true;
        }

        // Method to validate and filter CodingSession objects
        public static List<CodingSession> ValidateAndFilterSessions(List<CodingSession> sessions, out List<CodingSession> invalidSessions)
        {
            var validSessions = new List<CodingSession>();
            invalidSessions = new List<CodingSession>();

            if (sessions == null || sessions.Count == 0)
            {
                Log("[Validation] No sessions to validate.");
                return validSessions;
            }

            foreach (var session in sessions)
            {
                if (ValidateSession(session, out string errorMessage))
                {
                    validSessions.Add(session);
                }
                else
                {
                    invalidSessions.Add(session);
                    Log($"[Validation Error] {errorMessage}");
                }
            }

            return validSessions;
        }

        // Method to validate user input.
        public static string GetValidatedInput(string prompt, Func<string, bool> validation, string errorMessage)
        {
            string input;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine() ?? string.Empty;
                if (!validation(input))
                {
                    Console.WriteLine(errorMessage);
                }
            } while (!validation(input));

            return input;
        }

        // Method to validate ID input.
        public static int GetValidatedID(string prompt, Func<int, bool> validation, string errorMessage)
        {
            while (true)
            {
                try
                {
                    Console.Write(prompt);
                    string rawInput = Console.ReadLine()?.Trim() ?? string.Empty;

                    if (int.TryParse(rawInput, out int input) && validation(input))
                    {
                        return input; // Return the validated ID
                    }

                    Console.WriteLine(errorMessage); // Display validation error message
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid ID.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("The number is too large. Please enter a smaller ID.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An unexpected error occurred. Please try again.");
                    Log($"[Unexpected Error] {ex.Message}"); // Log the unexpected error
                }
            }
        }

        // Method to validate DateTime input.
        public static DateTime GetValidatedDateTime(string prompt)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(prompt);
                    string input = Console.ReadLine()?.Trim() ?? string.Empty;

                    if (DateTime.TryParseExact(input, "MM-dd-yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var dateTime))
                    {
                        return dateTime;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please use 'MM-dd-yyyy HH:mm'.");
                        Log($"[Validation Error] Invalid date input: {input}");
                    }
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    Log($"[Validation Error] FormatException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An unexpected error occurred. Please try again.");
                    Log($"[Unexpected Error] {ex.Message}");
                }
            }
        }
    }
}