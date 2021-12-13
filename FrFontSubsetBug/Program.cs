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
        private const string ReportName = "FontTest.frx";
        // private const string ReportName = "SimpleTest.frx";
        
        static async Task Main(string[] args)
        {
            await RenderReportsAsync(Enumerable.Range(0, 10));
            
            // RenderReports(Enumerable.Range(0, 10));
        }

        private static Task RenderReportsAsync(IEnumerable<int> range)
        {
            var tasks = range.Select(i => Task.Run(() => RenderReport(i)));
            
            return Task.WhenAll(tasks);
        }

        private static void RenderReports(IEnumerable<int> range)
        {
            foreach(var i in range)
            {
                RenderReport(i);
            }
        }
        
        private static void RenderReport(int i)
        {
            using var report = new Report();
            using var pdfExport = new PDFExport()
            {
                PdfCompliance = PDFExport.PdfStandard.PdfA_1a,
                EmbeddingFonts = true
            };

            var f = new FileInfo(Path.Combine("output", $"export_fr_embed{i}.pdf"));

            f.Directory!.Create();

            var ms = new MemoryStream();
            
            report.Load(ReportName);
            report.Prepare();
            report.Export(pdfExport, ms);
            
            File.WriteAllBytes(f.FullName, ms.ToArray());
        }
    }
}