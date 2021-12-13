namespace BuggedFontFinder.Folder
{
    public class FolderOutputWriter
    {
        public const string BasePath = "output";

        public string OutputFile(string relativePath, string fileName, byte[] content)
        {
            var file = new FileInfo(Path.Combine("output", relativePath, fileName));
            file.Directory!.Create();
            
            File.WriteAllBytes(file.FullName, content);

            return file.FullName;
        }
    }
}