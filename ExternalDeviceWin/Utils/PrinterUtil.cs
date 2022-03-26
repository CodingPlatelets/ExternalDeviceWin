using ExternalDeviceWin.Entites;
using PDFtoPrinter;
using System.Printing;
using System.Runtime.Versioning;
using System.Text;

namespace ExternalDeviceWin.Utils
{
    [SupportedOSPlatform("windows")]
    public static class PrinterUtil
    {
        private static readonly ILogger _logger;
        private static readonly PrintServer _server;

        static PrinterUtil()
        {
            _server = new LocalPrintServer();
            _logger = LogUtils.CreateLogger(nameof(PrinterUtil));
        }

        /// <summary>
        /// below will be overview
        /// OneNote (Desktop)
        /// Microsoft XPS Document Writer
        /// Microsoft Print to PDF
        /// Fax
        /// Adobe PDF
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetPrinterList()
        {
            // Todo: write to config file
            _logger.LogDebug(Thread.CurrentThread.ToString());
            using var myServer = new PrintServer();
            return from p in myServer.GetPrintQueues()
                   where !string.IsNullOrWhiteSpace(p.Name)
                   where !p.Name.Contains("OneNote (Desktop)")
                   where !p.Name.Contains("Microsoft Print to PDF")
                   where !p.Name.Contains("Fax")
                   where !p.Name.Contains("Adobe PDF")
                   where !p.Name.Contains("Microsoft XPS Document Writer")
                   select p.Name;


        }

        public static IEnumerable<PrintQueueInfo> GetSlefPrintQueuesInfo()
        {
            var printerList = GetPrinterList().ToList<string>();
            using var myServer = new PrintServer();
            using var pd = myServer.GetPrintQueues();
            return from p in pd
                   where printerList.Contains(p.Name)
                   select new PrintQueueInfo
                   {
                       Name = p.Name,
                       FullName = p.FullName,
                       IsAvailable = p.IsNotAvailable,
                       IsBusy = p.IsBusy,
                       IsInError = p.IsInError,
                       IsOffLine = p.IsOffline,
                       IsOutOfPapaer = p.IsOutOfPaper,
                       IsPaperJammed = p.IsPaperJammed,
                       IsPaused = p.IsPaused,
                       ErrMsg = PrinterErrMsg(p),
                   };

        }

        public static PrintQueueInfo? GetSlefPrintQueueInfo(string printQueueName)
        {

            using var p = new PrintServer().GetPrintQueue(printQueueName) ?? throw new NullReferenceException();

            return new PrintQueueInfo
            {
                Name = p.Name,
                FullName = p.FullName,
                IsAvailable = p.IsNotAvailable,
                IsBusy = p.IsBusy,
                IsInError = p.IsInError,
                IsOffLine = p.IsOffline,
                IsOutOfPapaer = p.IsOutOfPaper,
                IsPaperJammed = p.IsPaperJammed,
                IsPaused = p.IsPaused,
                ErrMsg = PrinterErrMsg(p),
            };
        }

        private static string PrinterErrMsg(PrintQueue pq)
        {
            var statusReport = new StringBuilder();
            if (pq.HasPaperProblem)
            {
                statusReport.Append("Has a paper problem. ");
            }
            if (!(pq.HasToner))
            {
                statusReport.Append("Is out of toner. ");
            }
            if (pq.IsDoorOpened)
            {
                statusReport.Append("Has an open door. ");
            }
            if (pq.IsInError)
            {
                statusReport.Append("Is in an error state. ");
            }
            if (pq.IsNotAvailable)
            {
                statusReport.Append("Is not available. ");
            }
            if (pq.IsOffline)
            {
                statusReport.Append("Is off line. ");
            }
            if (pq.IsOutOfMemory)
            {
                statusReport.Append("Is out of memory. ");
            }
            if (pq.IsOutOfPaper)
            {
                statusReport.Append("Is out of paper. ");
            }
            if (pq.IsOutputBinFull)
            {
                statusReport.Append("Has a full output bin. ");
            }
            if (pq.IsPaperJammed)
            {
                statusReport.Append("Has a paper jam. ");
            }
            if (pq.IsPaused)
            {
                statusReport.Append("Is paused. ");
            }
            if (pq.IsTonerLow)
            {
                statusReport.Append("Is low on toner. ");
            }
            if (pq.NeedUserIntervention)
            {
                statusReport.Append("Needs user intervention. ");
            }
            return statusReport.ToString();
        }

        public static bool ExecutePdf(Stream ms, string printerName, string? Page)
        {
            if (string.IsNullOrEmpty(Page))
            {
                Page = "";
            }

            var path = FileUtils.saveFile(ms, null,FileUtils.GetStreamExtension(ms));
            if (!string.IsNullOrEmpty(path))
            {
                var wrapper = new PDFtoPrinterPrinter();
                var op = new PrintingOptions(printerName,path);
                op.Pages = Page;
                wrapper.Print(op).Wait();
                FileUtils.DeleteFile(path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
