using ExternalDeviceWin.Utils;
using Grpc.Core;
using System.Net;
using System.Runtime.Versioning;

namespace ExternalDeviceWin.Services
{
    [SupportedOSPlatform("windows")]
    public class PrinterInfoService: PrinterInfo.PrinterInfoBase
    {
        private readonly ILogger<PrinterInfoService> _logger;
        private readonly PrinterUtil _printerUtils;
        public PrinterInfoService(ILogger<PrinterInfoService> logger, ILogger<PrinterUtil> logger2)
        {
            _logger = logger;
            _printerUtils = new PrinterUtil(logger2);
        }
        public override Task<PrinterInfoResponse> GetAvailablePrinters(PrinterInfoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{0} is sending a requset to get printerInfo", request.OriginLinuxName);
            var targetPrinter = request.OriginPrinterName;
            var printerList = _printerUtils.GetPrinterList();
            var isPrinterExisted = false;
            if (printerList.Contains(targetPrinter))
            {
                isPrinterExisted = true;
            }
            if (isPrinterExisted)
            {

            }
            var resp = new PrinterInfoResponse();
            resp.IsPrinterExisted = isPrinterExisted;
            resp.AvailablePrinter.AddRange(printerList);
            resp.WinName = Environment.MachineName;
            return Task.FromResult(resp);
        }


        public override async Task<FileResp> FileHandler(IAsyncStreamReader<FileReq> requestStream, ServerCallContext context)
        {
            var resp = new FileResp();
            while (await requestStream.MoveNext())
            {
                var c = requestStream.Current;
                _logger.LogInformation("{} send a file to print", c.OriginLinuxName);
                using var ms = new MemoryStream(c.Files.ToByteArray());
                //todo: print the file
            }
            return new FileResp
            {
                Success = new Success
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "file upload is succeeded",
                }
            };

        }
    }
}
