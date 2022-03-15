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

        public override Task<PrintQueueRep> GetPrintQueueInfo(PrintQueueReq request, ServerCallContext context)
        {
            var resp = new PrintQueueRep();
            if (string.IsNullOrEmpty(request.PrintQueueName))
            {
                resp.PrintQueueInfoList.PrintQueueList.AddRange(from p in PrinterUtil.GetSlefPrintQueuesInfo()
                                                                select new printQueueInfo
                                                                {
                                                                    Name = p.Name,
                                                                    FullName = p.FullName,
                                                                    IsAvailable = p.IsAvailable,
                                                                    IsBusy = p.IsBusy,
                                                                    IsInError = p.IsInError,
                                                                    IsOffLine = p.IsOffLine,
                                                                    IsOutOfPaper = p.IsOutOfPapaer,
                                                                    IsPaperJammed = p.IsPaperJammed,
                                                                    IsPaused = p.IsPaused,
                                                                    ErrMsg = p.ErrMsg,
                                                                });
            }
            else
            {
                var p = PrinterUtil.GetSlefPrintQueueInfo(request.PrintQueueName);
                if(p is null)
                {
                    resp.PrintQueueInfo = new printQueueInfo
                    {
                        Name = default(string),
                        FullName = default(string),
                        IsAvailable = default(bool),
                        IsBusy = default(bool),
                        IsInError = default(bool),
                        IsOffLine = default(bool),
                        IsOutOfPaper = default(bool),
                        IsPaperJammed = default(bool),
                        IsPaused = default(bool),
                    };
                }
                else
                {
                    resp.PrintQueueInfo = new printQueueInfo
                    {
                        Name = p.Name,
                        FullName = p.FullName,
                        IsAvailable = p.IsAvailable,
                        IsBusy = p.IsBusy,
                        IsInError = p.IsInError,
                        IsOffLine = p.IsOffLine,
                        IsOutOfPaper = p.IsOutOfPapaer,
                        IsPaperJammed = p.IsPaperJammed,
                        IsPaused = p.IsPaused,
                        ErrMsg = p.ErrMsg,
                    } ?? throw new NullReferenceException();
                }
            }

            return Task.FromResult(resp) ;
        }
    }
}
