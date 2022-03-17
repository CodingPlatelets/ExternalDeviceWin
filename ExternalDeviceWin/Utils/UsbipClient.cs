using Grpc.Core;
using System.Diagnostics;
using System.Text;

namespace ExternalDeviceWin.Utils
{
    public class UsbipClient
    {
        private readonly ILogger<UsbipClient> _logger;
        private string executeFilePath
        {
            get;
            init;
        }
        private string workingDir
        {
            get;
            init;
        }
        public UsbipClient (string executePath = "usbip")
        {
            _logger = new LoggerFactory().CreateLogger<UsbipClient>();
            executeFilePath = executePath;
            workingDir = Directory.GetCurrentDirectory() + @"\ExternalDepends";
        }

        public Tuple<string,bool> InitUsbConnect(string busId, string serverIPAddress, ServerCallContext ctx)
        {
            var msg = $"{busId} is Connected";
            var errMsg = new StringBuilder();
            try
            {
                Directory.SetCurrentDirectory(workingDir);
                if (!checkExcuteFile())
                {
                    _logger.LogError("without usbip exe");
                    return new Tuple<string, bool>(msg, false);
                }
                using var p = new Process();
                //TODO need search first() search still need 25s, or it will wait for at least 25s
                var startInfo = new ProcessStartInfo(executeFilePath, $"attach -r {serverIPAddress} -b {busId}");
                startInfo.UseShellExecute = false;              //不显示shell
                startInfo.CreateNoWindow = true;                //不创建窗口
                startInfo.RedirectStandardError = true;         //打开错误流
                p.EnableRaisingEvents = true;
                p.StartInfo = startInfo;

                p.ErrorDataReceived += (sender, c) =>
                {
                    Console.WriteLine("error");
                    Console.WriteLine(c.Data);
                    errMsg.Append(c.Data);
                };

                p.Start();

                p.BeginErrorReadLine();
                //p.BeginOutputReadLine();

                p.WaitForExit(25000);
                p.Close();
            }
            finally
            {
                Directory.SetCurrentDirectory("..");
            }
            return string.IsNullOrEmpty(errMsg.ToString()) ? new Tuple<string,bool>(msg,true) : new Tuple<string,bool>(errMsg.ToString(),false);
        }

        private bool checkExcuteFile() => File.Exists("usbip.exe") && File.Exists("attacher.exe");

        private bool checkDeviceExport(string busId, string serverIPAddress, ServerCallContext ctx)
        {
            var p = new Process();
            var errMsg = new StringBuilder();
            var outputMsg = new StringBuilder();

            var startInfo = new ProcessStartInfo(executeFilePath, $"list -r {serverIPAddress}");
            startInfo.RedirectStandardError=true;
            startInfo.RedirectStandardOutput = true;
            p.EnableRaisingEvents = true;
            p.StartInfo = startInfo;
            using var outputWaitHandler = new AutoResetEvent(false);
            using var errorWaitHandler = new AutoResetEvent(false);

            p.OutputDataReceived += (sender, c) =>
            {
                if(c.Data is null)
                {
                    outputWaitHandler.Set();
                }
                else
                {
                    outputMsg.AppendLine(c.Data);
                }
            };

            p.ErrorDataReceived += (sender, c) =>
            {
                if (c.Data is null)
                {
                    errorWaitHandler.Set();
                }
                else
                {
                    errMsg.AppendLine(c.Data);
                }
            };

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            if(p.WaitForExit(25000) && outputWaitHandler.WaitOne(1000) && errorWaitHandler.WaitOne(1000))
            {
                
            }
            else
            {

            }
            _logger.LogDebug("output : {}", outputMsg.ToString());
            _logger.LogDebug("error : {}", errMsg.ToString());
            return false;
        }

    }
}
