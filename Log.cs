using System;
using System.IO;
using Win_Labs;

namespace Win_Labs
{
    public static class Log
    {
        private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string logFileName = $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"; // Log file with date and time
        private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);
        private static readonly int maxLogFiles = 10; // Keep only the last 10 log files
        public static void logFile()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                log($"Log Directory created at: {logDirectory}");

            }
            // Ensure the log file exists
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
                log($"Log File created at: {logFilePath}");
            }
            // Initialize the log file
            File.AppendAllText(logFilePath, $"Log started at {DateTime.Now}\n");

            // Remove old log files if more than 10 exist
            CleanUpOldLogFiles();
        }
        // Method to clean up old log files, keeping only the last "maxLogFiles"
        private static void CleanUpOldLogFiles()
        {
            var logFiles = Directory.GetFiles(logDirectory, "log_*.txt")
                                    .OrderByDescending(f => File.GetCreationTime(f))
                                    .Skip(maxLogFiles)
                                    .ToList();

            foreach (var oldLogFile in logFiles)
            {
                try
                {
                    File.Delete(oldLogFile);
                }
                catch (Exception ex)
                {
                    log($"Error deleting old log file {oldLogFile}: {ex.Message}", Log.LogLevel.Error);
                }
            }
        }
        public static void log(string message, LogLevel level = LogLevel.Info)
        {
            try
            {
                var logMessage = $"{DateTime.Now:yy-MM-dd HH:mm:ss} [{level}] {message}";
                Console.WriteLine(logMessage);
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Handle exceptions related to logging
                Console.WriteLine($"ERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nLogging failed: {ex.Message} \nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR");
            }
        }

        public static void logException(Exception ex)
        {
            log($"Exception: {ex.Message}\n{ex.StackTrace}", LogLevel.Error);
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            UhOh
        }
    }
}
