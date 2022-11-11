using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FrFontSubsetBug;

public class GhostScriptUtils
{
    private readonly string _outputPath;
    private const string GhostScriptPathEnvVarName = "GHOSTSCRIPT_PATH";

    private readonly string? _gsPath;
    private static ReadOnlySpan<string> PdfToPclArgs => new[]
    {
            "-q", // Quiet startup: suppress normal startup messages
            "-dSAFER", // Enables access controls on files
            "-dBATCH", // Causes Ghostscript to exit after processing all files
            "-dNOPAUSE", // Disables the prompt and pause at the end of each page
        };

    public GhostScriptUtils(string outputPath)
    {
        _outputPath = outputPath;
        _gsPath = Environment.GetEnvironmentVariable(GhostScriptPathEnvVarName);
    }

    public async Task<string> ConvertPdfAsync(FileInfo pdfFile, string outputDevice, int dpiResolution)
    {
        if (_gsPath == null)
        {
            throw new ArgumentNullException(
                $"{GhostScriptPathEnvVarName} environment variable has not been set. Set its value to a path leading to the ghoscript executable.");
        }

        var fileName = pdfFile.Name.Split('.')[0];

        var outputFile = Path.Combine(_outputPath, $"{fileName}.pcl");

        var logPath = Path.Combine(_outputPath, "gs_log.txt");
        
        var args = new List<string>(PdfToPclArgs.ToArray())
            {
                $"-sstdout={logPath}",
                $"-sDEVICE={outputDevice}",
                $"-r{dpiResolution}",
                @"-sOutputFile=" + outputFile,
                @"-f",
                pdfFile.FullName
            };

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo(_gsPath, string.Join(' ', args))
        };

        process.Start();
        
        await process.WaitForExitAsync();

        var logContents = await File.ReadAllTextAsync(logPath);

        return logContents;
    }
}