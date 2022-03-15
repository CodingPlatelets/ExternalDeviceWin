using Grpc.Core;
using System.Diagnostics;

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
            var msg = string.Empty;
            Directory.SetCurrentDirectory(workingDir);
            if (!checkExcuteFile())
            {
                _logger.LogError("without usbip exe");
                return new Tuple<string,bool>(msg,false);
            }
            using var p = new Process();
            var startInfo = new ProcessStartInfo(executeFilePath, $"attach -r {serverIPAddress} -b {busId}");
            //startInfo.UseShellExecute = false;          //不显示shell
            //startInfo.CreateNoWindow = true;            //不创建窗口
            startInfo.RedirectStandardOutput = true;    //打开流输出
            startInfo.RedirectStandardError = true;     //打开错误流
            p.StartInfo = startInfo;

            p.Start();

            var errMsg = p.StandardError.ReadToEnd();
            msg = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            p.Close();
            Directory.SetCurrentDirectory("..");
            return string.IsNullOrEmpty(errMsg) ? new Tuple<string,bool>(msg,true) : new Tuple<string,bool>(errMsg,false);
        }

        private bool checkExcuteFile() => File.Exists("usbip.exe") && File.Exists("attacher.exe");



    }
}
