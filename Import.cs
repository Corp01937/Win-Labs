using System.IO.Compression;
using System.IO;
using System.Windows;

namespace Win_Labs
{
    internal class import
    {
        public static void openZIP(string importPath, string exportPath)
        {
            Log.Info("Opening file: " + importPath);

            try
            {
                Log.Info($"Creating playlist at {exportPath}");

                // Check if the export directory exists
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                    Log.Info("Export path created: " + exportPath);
                }

                // Flags for overwrite/skip all options
                bool overwriteAll = false;
                bool skipAll = false;

                // Extract files with overwrite handling
                using (var archive = System.IO.Compression.ZipFile.OpenRead(importPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(exportPath, entry.FullName);

                        // Check if the file already exists
                        if (File.Exists(destinationPath))
                        {
                            if (skipAll)
                            {
                                Log.Info($"Skipped file (skip all): {entry.FullName}");
                                continue; // Skip all remaining duplicates
                            }

                            if (!overwriteAll)
                            {
                                var result = MessageBox.Show(
                                    $"The file '{entry.FullName}' already exists. Overwrite?",
                                    "File Exists",
                                    MessageBoxButton.YesNoCancel,
                                    MessageBoxImage.Warning);

                                if (result == MessageBoxResult.No)
                                {
                                    Log.Info($"Skipped file: {entry.FullName}");
                                    continue; // Skip this file
                                }
                                else if (result == MessageBoxResult.Cancel)
                                {
                                    Log.Info("User canceled extraction.");
                                    return; // Stop extraction
                                }
                                else if (result == MessageBoxResult.Yes)
                                {
                                    // Ask if this action should apply to all remaining duplicates
                                    var applyToAll = MessageBox.Show(
                                        "Apply this action to all remaining files?",
                                        "Apply to All",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

                                    if (applyToAll == MessageBoxResult.Yes)
                                    {
                                        overwriteAll = true; // Set overwrite all flag
                                    }
                                }
                            }
                        }

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        // Extract the file
                        entry.ExtractToFile(destinationPath, true); // Overwrite if allowed
                        Log.Info($"Extracted file: {entry.FullName}");
                    }
                }

                Log.Info($"{exportPath} successfully populated with files from the ZIP archive.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error extracting ZIP file: {ex.Message}");
                MessageBox.Show($"Could not open file {importPath}. Please check the location or permissions.",
                    "File Opening Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
