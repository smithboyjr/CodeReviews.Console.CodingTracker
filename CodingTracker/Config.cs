using Microsoft.Extensions.Configuration;
using static CodingTracker.LogHandler;

namespace CodingTracker
{
    public static class Config
    {
        private static readonly IConfigurationRoot Configuration;

        static Config()
        {
            try
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Log($"[Config Error] Failed to load configuration: {ex.Message}");
                throw;
            }
        }

        public static string GetConnectionString()
        {
            var connString = Configuration.GetConnectionString("CodingTrackerDb"); // Must match JSON key
            if (string.IsNullOrEmpty(connString))
            {
                throw new Exception("[Config Error] Connection string is null or empty.");
            }
            return connString;
        }
    }
}