using System.Drawing.Printing;
using System.Runtime.Versioning;

namespace ExternalDeviceWin.Utils
{
    [SupportedOSPlatform("windows")]
    public class PrinterUtil
    {
        private readonly ILogger<PrinterUtil> _logger;


        public PrinterUtil(ILogger<PrinterUtil> logger)
        {
            _logger = logger;
        }

        
        public IEnumerable<string> GetPrinterList()
        {
            _logger.LogInformation("get printer list");
            var l = new List<string>();
            foreach(string i in PrinterSettings.InstalledPrinters)
            {
                l.Add(i);
            }
            return l;
            
        }

        public string GetPrinterInfo(string printerName)
        {
            if (!GetPrinterList().ToList().Contains(printerName))
            {
                return string.Empty;
            }
            //todo: get printer info
            return "OK";
        }
    }
}
