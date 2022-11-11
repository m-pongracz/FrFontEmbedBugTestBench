using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastReport;
using FastReport.Export.Pdf;

namespace FrFontSubsetBug
{
    class Program
    {
        // private const string ReportName = "FontTest.frx";
        private const string ReportName = "SimpleTest.frx";

        private const string OutputPath = "output";
        
        static async Task<int> Main(string[] args)
        {
            var docsNum = 100;
            
            var runConf = args[0];

            switch (runConf)
            {
                case "single":
                    await RenderReportsAsync(Enumerable.Range(0, docsNum));
                    break;
                case "multi":
                    await RenderReportsParallel(Enumerable.Range(0, docsNum));
                    break;
                case "pcl":
                    var files = await RenderReportsParallel(Enumerable.Range(0, docsNum));

                    var logLines = new List<string>();
                    
                    var gs = new GhostScriptUtils(OutputPath);    
                    
                    foreach (var f in files)
                    {
                        var log = await gs.ConvertPdfAsync(f, "ljet4", 100);

                        if (!string.IsNullOrEmpty(log))
                        {
                            logLines.Add($"{f.Name}: {log}");
                        }
                    }

                    await File.WriteAllLinesAsync(Path.Combine(OutputPath, "complete_gs_log.txt"), logLines);
                    
                    break;
            }
            
            return 0;
        }

        private static async Task<IEnumerable<FileInfo>> RenderReportsParallel(IEnumerable<int> range)
        {
            var files = new List<FileInfo>();
            
            await Parallel.ForEachAsync(range, async (i, _) =>
            {
                var f = await RenderReport(i);
                
                files.Add(f);
            });

            return files;
        }

        private static async Task RenderReportsAsync(IEnumerable<int> range)
        {
            foreach(var i in range)
            {
                await RenderReport(i);
            }
        }
        
        private static async Task<FileInfo> RenderReport(int i)
        {
            Console.WriteLine($"Rendering: {i}");
            
            using var report = new Report();
            using var pdfExport = new PDFExport()
            {
                PdfCompliance = PDFExport.PdfStandard.PdfA_1a,
                EmbeddingFonts = true
            };

            var f = new FileInfo(Path.Combine(OutputPath, $"export_fr_embed{i}.pdf"));

            f.Directory!.Create();

            var ms = new MemoryStream();
            
            report.Load(ReportName);
            report.Prepare();
            report.Export(pdfExport, ms);
            
            await File.WriteAllBytesAsync(f.FullName, ms.ToArray());
            
            Console.WriteLine($"Finished rendering: {i}");

            return f;
        }
    }
}