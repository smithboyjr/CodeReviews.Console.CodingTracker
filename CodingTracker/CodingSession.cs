namespace CodingTracker
{
    public class CodingSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Duration { get; set; }

        // Calculate the duration of the coding session
        public void CalculateDuration()
        {
            if (EndTime > StartTime)
            {
                Duration = (EndTime - StartTime).ToString(); // Calculate the duration
            }
            else
            {
                throw new InvalidOperationException("End time must be after start time.");
            }
        }
    }
}