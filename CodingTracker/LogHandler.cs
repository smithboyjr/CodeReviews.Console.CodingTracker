namespace CodingTracker
{
    public static class LogHandler
    {
        // Success/Error Messages Constants
        public const string SessionNotFoundMessage = "Error: No session found with the specified Id.";
        public const string SessionInsertedMessage = "Success: Session inserted successfully.";
        public const string SessionUpdatedMessage = "Success: Session updated successfully.";
        public const string SessionDeletedMessage = "Success: Session deleted successfully.";
        public const string SessionDisplayedMessage = "Success: Session displayed successfully.";
        public const string DatabaseErrorMessage = "An error occurred while accessing the database. Please try again later.";
        public const string UnexpectedErrorMessage = "An unexpected error occurred. Please contact support.";
        public const string NoSessionsFoundMessage = "No sessions found.";

        // Method to log success/error messages
        public static void Log(string message)
        {
            Console.WriteLine($"[Log] {DateTime.Now}: {message}");
        }
    }
}