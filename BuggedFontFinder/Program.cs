namespace BuggedFontFinder.Folder;

class Program
{
    static async Task Main(string[] args)
    {
        var folderPath = args[0];

        var files = Directory
            .GetFiles(folderPath)
            .Where(f => f.EndsWith(".pdf"))
            .Select(f => new FileInfo(f))
            .ToArray();

        var processingService = new FileProcessingService();

        
        const int pageSize = 10;

        for (var i = 0; i < files.Length; i+=pageSize)
        {
            var batch = files.Skip(i).Take(pageSize);
            
            await processingService.ProcessFiles(batch, new DirectoryInfo(Path.Combine(folderPath, "invalid")));
        }
    }
}