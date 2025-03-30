using System;
using System.Configuration;
using System.Collections.Specialized;

namespace CodingTracker
{
    public class ConConfig
    {
        public static void ReadConfig()
        {
            try
            {
                Console.WriteLine("Loading configuration file...");

                // Explicitly load the configuration file
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config.AppSettings.Settings.Count == 0)
                {
                    Console.WriteLine("No keys found in AppSettings. Configuration file may be empty.");
                    return;
                }

                Console.WriteLine("Configuration file loaded successfully. Displaying keys and values:");

                foreach (KeyValueConfigurationElement setting in config.AppSettings.Settings)
                {
                    Console.WriteLine($"Key: {setting.Key}, Value: {setting.Value}");
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine($"Error loading configuration file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}