using ExternalDeviceWin.Entites;
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
            using var ms = new MemoryStream();
            var printerName = string.Empty;
            var pages = string.Empty;
            while (await requestStream.MoveNext())
            {
                var c = requestStream.Current;
                printerName = c.PrinterName;
                pages = c.Pages;
                _logger.LogInformation("{} send a file to print", c.OriginLinuxName);
                using var temp = new MemoryStream(c.Files.ToArray());
                await temp.CopyToAsync(ms).ConfigureAwait(false);
            }
            ms.Position = 0;
            if (string.IsNullOrEmpty(printerName))
            {
                return new FileResp
                {
                    Error = new Error
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "you must define printer name",
                    }
                };
            }
            if (PrinterUtil.ExecutePdf(ms, printerName, pages))
            {
                return new FileResp
                {
                    Success = new Success
                    {
                        Code = (int)HttpStatusCode.OK,
                        Message = "file upload is succeeded",
                    }
                };
            }
            else
            {
                return new FileResp
                {
                    Error = new Error
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "file upload wrong",
                    }
                };

            }
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
