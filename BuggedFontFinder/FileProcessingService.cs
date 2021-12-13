using System.Diagnostics;

namespace BuggedFontFinder.Folder;

public class FileProcessingService
{
    public async Task ProcessFiles(IEnumerable<FileInfo> files, DirectoryInfo outputDir)
    {
        var processTasks = files.Select(async fi =>
        {
            var valid = await ProcessFile(fi);

            if (valid)
            {
                Console.WriteLine($"File {fi.Name} is valid");
            }
            else
            {
                Directory.CreateDirectory(outputDir.FullName);

                File.Copy(fi.FullName, Path.Combine(outputDir.FullName, fi.Name), true);

                Console.WriteLine($"File {fi.Name} is invalid");
            }
        }).ToArray();

        await Task.WhenAll(processTasks);
    }

    private async Task<bool> ProcessFile(FileInfo fileInfo)
    {
        var dir = Directory.GetCurrentDirectory();
        var jarPath = Path.Combine(dir, "lib", "PdfPreflight.jar");

        var pdfArgs = new List<string>()
        {
            "-jar",
            jarPath,
            fileInfo.FullName
        };
        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo($"java", string.Join(' ', pdfArgs))
            {
            }
        };

        process.Start();

        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }
}