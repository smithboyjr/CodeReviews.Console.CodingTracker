namespace CodingTracker
{
    public static class UserInput
    {
        // Method to handle Date and Time input from the user
        public static DateTime GetDateTime(string prompt)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine() ?? string.Empty;
            return Validation.GetValidatedDateTime(input);
        }

        // Method to handle Menu and ID input from the user
        public static string GetInput(string prompt, Func<string, bool> validation, string errorMessage)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!validation(input))
                {
                    Console.WriteLine(errorMessage);
                }
            } while (!validation(input));

            return input;
        }
    }
}