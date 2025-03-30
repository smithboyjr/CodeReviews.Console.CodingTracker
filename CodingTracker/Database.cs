using System;
using System.Configuration;
using Microsoft.Data.Sqlite;
using static CodingTracker.LogHandler;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using static CodingTracker.Validation;
using static CodingTracker.CodingSession;

namespace CodingTracker
{
    public static class Database
    {
        // SQL Query Constants
        private const string InsertQuery = "INSERT INTO coding_tracker (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration)";
        private const string UpdateQuery = "UPDATE coding_tracker SET StartTime = @StartTime, EndTime = @EndTime, Duration = @Duration WHERE Id = @Id";
        private const string DeleteQuery = "DELETE FROM coding_tracker WHERE Id = @Id";
        private const string SelectAllQuery = "SELECT Id, StartTime, EndTime, Duration FROM coding_tracker";

        // Method to retrieve the connection string from App.config with exception handling
        public static string GetConnectionString()
        {
            try
            {
                string? connectionString = ConfigurationManager.ConnectionStrings["CodingTrackerDb"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'CodingTrackerDb' is missing or empty in App.config.");
                }

                return connectionString;
            }
            catch (ConfigurationErrorsException ex)
            {
                Log($"[Error] Failed to load connection string due to configuration error: {ex.Message}");
                throw new InvalidOperationException("A configuration error occurred while retrieving the connection string.", ex);
            }
            catch (Exception ex)
            {
                Log($"[Error] Unexpected error while retrieving connection string: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while retrieving the connection string.", ex);
            }
        }

        // Thia method inserts a new coding session into the database
        public static void InsertSessionToDatabase(CodingSession session)
        {
            try
            {
                // Ensure the session has a valid duration
                session.CalculateDuration();

                ExecuteNonQuery(
                    InsertQuery,
                    command =>
                    {
                        command.Parameters.AddWithValue("@StartTime", session.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@EndTime", session.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Duration", session.Duration);
                    }
                );

                Log("Session successfully inserted into the database.");
            }
            catch (Exception ex)
            {
                Log($"[Error] Failed to insert session. Error: {ex.Message}");
                Console.WriteLine("An error occurred while inserting the session.");
            }
        }

        // This method deletes a specific coding session by its ID from the database
        public static void DeleteSessionFromDatabase(int sessionId)
        {
            try
            {
                ExecuteNonQuery(
                    DeleteQuery,
                    command =>
                    {
                        command.Parameters.AddWithValue("@Id", sessionId);
                    },
                    rowsAffected =>
                    {
                        if (rowsAffected == 0)
                        {
                            Console.WriteLine($"No session found with ID {sessionId}. Nothing was deleted.");
                        }
                        else
                        {
                            Console.WriteLine($"Session with ID {sessionId} successfully deleted.");
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                Log($"[Error] Failed to delete session with ID {sessionId}. Error: {ex.Message}");
                Console.WriteLine("An error occurred while deleting the session.");
            }
        }

        // This method updates a specific coding session by its ID in the database
        public static void UpdateSessionInDatabase(CodingSession session)
        {
            try
            {
                // Ensure the session has a valid duration
                session.CalculateDuration(); 

                ExecuteNonQuery(
                    UpdateQuery,
                    command =>
                    {
                        command.Parameters.AddWithValue("@Id", session.Id);
                        command.Parameters.AddWithValue("@StartTime", session.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@EndTime", session.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Duration", session.Duration);
                    }
                );

                Log($"Session with ID {session.Id} successfully updated.");
            }
            catch (Exception ex)
            {
                Log($"[Error] Failed to update session with ID {session.Id}. Error: {ex.Message}");
                Console.WriteLine("An error occurred while updating the session.");
            }
        }

        // This method initializes the database by creating the coding_tracker table if it doesn't exist
        public static void InitializeDatabase()
        {
            Log("Initializing database...");
            try
            {
                using (var connection = new SqliteConnection(GetConnectionString()))
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();

                    tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS coding_tracker(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    Duration TEXT NOT NULL)";
                    tableCmd.ExecuteNonQuery();

                    Log("Database initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                Log($"[Error] Failed to initialize database. Error: {ex.Message}");
                throw; // Re-throw the exception to stop execution if the database cannot be initialized
            }
        }

        // This method executes a query command (SELECT) and processes the results with exception handling
        public static void ExecuteReader(string commandText, Action<SqliteCommand>? configureCommand, Action<SqliteDataReader> processRows)
        {
            SqliteCommand? command = null; // Declare command outside the try block
            try
            {
                using (var connection = new SqliteConnection(GetConnectionString()))
                {
                    connection.Open();
                    command = connection.CreateCommand(); // Initialize command
                    command.CommandText = commandText;

                    // Configure parameters securely if provided
                    configureCommand?.Invoke(command);

                    Log($"Executing query: {commandText}");
                    using (var reader = command.ExecuteReader())
                    {
                        Log("Query executed successfully. Processing rows...");
                        processRows(reader); // Process the rows using the provided callback
                    }
                }
            }
            catch (SqliteException ex)
            {
                string parameters = command != null ? GetCommandParameters(command) : "No parameters";
                Log($"[Database Error] Query: {commandText} | Parameters: {parameters} | Error: {ex.Message}");
                Console.WriteLine("[Database Error] An error occurred while executing the query.");
            }
            catch (Exception ex)
            {
                Log($"[Unexpected Error] Query: {commandText} | Error: {ex.Message}");
                Console.WriteLine("[Unexpected Error] An unexpected error occurred.");
            }
        }

        // Reusable method to execute non-query commands (INSERT, UPDATE, DELETE)
        public static void ExecuteNonQuery(string commandText, Action<SqliteCommand> configureCommand, Action<int>? handleRowsAffected = null)
        {
            SqliteCommand? command = null;
            try
            {
                using (var connection = new SqliteConnection(GetConnectionString()))
                {
                    connection.Open();
                    command = connection.CreateCommand();
                    command.CommandText = commandText;

                    // Configure parameters securely
                    configureCommand(command);

                    // Execute the command and get the number of rows affected
                    int rowsAffected = command.ExecuteNonQuery();

                    // Optional callback to handle rows affected
                    handleRowsAffected?.Invoke(rowsAffected);
                }
            }
            catch (Exception ex)
            {
                string parameters = command != null ? GetCommandParameters(command) : "No parameters";
                Log($"[Database Error] Query: {commandText} | Parameters: {parameters} | Error: {ex.Message}");
                Console.WriteLine("An error occurred while executing the query.");
            }
        }

        // Helper method to retrieve query parameters for logging
        private static string GetCommandParameters(SqliteCommand command)
        {
            return string.Join(", ", command.Parameters.Cast<SqliteParameter>().Select(p => $"{p.ParameterName}={p.Value}"));
        }

        // This method retrieves all coding sessions from the database and returns them as a list of CodingSession objects
        public static List<CodingSession> GetAllSessions()
        {
            var sessions = new List<CodingSession>();

            try
            {
                ExecuteReader(
                    SelectAllQuery,
                    null, // No parameters to configure
                    reader =>
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                // Parse and add each session
                                sessions.Add(new CodingSession
                                {
                                    Id = reader.GetInt32(0),
                                    StartTime = DateTime.Parse(reader.GetString(1)),
                                    EndTime = DateTime.Parse(reader.GetString(2)),
                                    Duration = reader.GetString(3)
                                });
                            }
                            catch (Exception ex)
                            {
                                Log($"[Warning] Failed to parse session from database row. Error: {ex.Message}");
                            }
                        }
                    }
                );

                // Validate sessions using the Validation class
                var validSessions = ValidateAndFilterSessions(sessions, out var invalidSessions);

                // Inform the user about invalid sessions
                if (invalidSessions.Count > 0)
                {
                    var invalidIds = string.Join(", ", invalidSessions.Select(s => s.Id));
                    Console.WriteLine($"[Warning] {invalidSessions.Count} invalid sessions were found and excluded (IDs: {invalidIds}). Check logs for details.");
                }

                Log($"[Info] Retrieved {validSessions.Count} valid sessions from the database.");
                return validSessions;
            }
            catch (SqliteException ex)
            {
                Log($"[Database Error] Failed to retrieve sessions. Error: {ex.Message}");
                Console.WriteLine("[Database Error] An error occurred while retrieving sessions.");
            }
            catch (Exception ex)
            {
                Log($"[Unexpected Error] Failed to retrieve sessions. Error: {ex.Message}");
                Console.WriteLine("[Unexpected Error] An unexpected error occurred while retrieving sessions.");
            }

            return new List<CodingSession>(); // Return an empty list to avoid crashing
        }
    }
}
