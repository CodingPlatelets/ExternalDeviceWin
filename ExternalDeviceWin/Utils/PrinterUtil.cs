using ExternalDeviceWin.Entites;
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
            _server = new PrintServer();
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
            _logger.LogDebug("get printer list");
            // Todo: write to config file
            return from p in _server.GetPrintQueues()
                   where !string.IsNullOrWhiteSpace(p.Name)
                   where !p.Name.Contains("OneNote (Desktop)")
                   where !p.Name.Contains("Microsoft Print to PDF")
                   where !p.Name.Contains("Fax")
                   where !p.Name.Contains("Adobe PDF")
                   where !p.Name.Contains("Microsoft XPS Document Writer")
                   select p.Name;
        }

        public static string GetPrinterInfo(string printerName)
        {
            var ll = GetPrinterList().ToList<string>();
            if (!GetPrinterList().ToList().Contains(printerName))
            {
                return string.Empty;
            }
            using var printQueues = _server.GetPrintQueues();
            var l = from p in printQueues where ll.Contains(p.Name) select p;
            Console.WriteLine(from lll in l select new { lll.Name, lll.FullName, lll.Description, lll.DefaultPrintTicket, lll.QueuePort, lll.UserPrintTicket });
            return "OK";
        }

        public static IEnumerable<PrintQueueInfo> GetSlefPrintQueuesInfo()
        {
            var printerList = GetPrinterList().ToList<string>();
            using var pd = _server.GetPrintQueues();
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
            using var pd = _server.GetPrintQueues();
            var l = GetSlefPrintQueuesInfo();
            if(!l.Any())
            {
                return default(PrintQueueInfo);
            }
            return l.FirstOrDefault(p => printQueueName == p.Name);
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

        private static bool ExecutePdf(Stream ms, string printerName, bool isColored,
            int fileCout, int beginPage = 0, int endPage = 0, string pageSize = "A4")
        {
            return true;
        }
    }
}
