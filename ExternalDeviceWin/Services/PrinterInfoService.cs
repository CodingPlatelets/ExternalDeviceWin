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
        public PrinterInfoService(ILogger<PrinterInfoService> logger)
        {
            _logger = logger;
        }

        public override Task<PrinterInfoResponse> GetAvailablePrinter(PrinterInfoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{} is sending a requset to get printerInfo", request.OriginLinuxName);
            var printerList = PrinterUtil.GetPrinterList();
            var resp = new PrinterInfoResponse();
            resp.AvailablePrinter.AddRange(printerList);
            resp.WinName = Environment.UserName;
            return Task.FromResult(resp);
        }


        public override async Task<FileResp> FileHandler(IAsyncStreamReader<FileReq> requestStream, ServerCallContext context)
        {
            var resp = new FileResp();
            while (await requestStream.MoveNext())
            {
                var c = requestStream.Current;
                _logger.LogInformation("{} send a file to print", c.OriginLinuxName);
                await using var ms = new MemoryStream(c.Files.ToByteArray());
                //TODO: print the file
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
