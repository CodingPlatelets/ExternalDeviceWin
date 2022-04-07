using ExternalDeviceWin.Utils;
using Grpc.Core;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ExternalDeviceWin.Services
{
    public class NetworkService : NetworkConnecter.NetworkConnecterBase
    {
        private readonly ILogger<NetworkService> _logger;

        public NetworkService(ILogger<NetworkService> logger)
        {
            _logger = logger;
        }

        private static string GetLocalIPv4(NetworkInterfaceType _type)
        {
            var output = string.Empty;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }

            return output;
        }

        public override Task<AddressResp> GetWinAdd(AddressReq request, ServerCallContext context)
        {
            if (!Enum.IsDefined(typeof(NetworkInterfaceType), request.NetworkType))
            {
                return Task.FromResult(new AddressResp
                {
                    Ipv4Add = string.Empty,
                    Error = new Error
                    {
                        Code = (int) HttpStatusCode.BadRequest,
                        Message = "your network type is wrong",
                    }
                });
            }

            return Task.FromResult(new AddressResp
            {
                Ipv4Add = GetLocalIPv4((NetworkInterfaceType) request.NetworkType),
                Success = new Success
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "OK",
                }
            });
        }

        public override Task<UsbipCallResp> InitUsbIp(UsbipDevice request, ServerCallContext context)
        {
            var client = new UsbipClient();
            var msg = client.InitUsbConnect(request.BusID, request.OriginIpAddress, context);
            //var msg = new Tuple<string, bool>("connect", true);
            var rep = new UsbipCallResp();
            if (msg.Item2)
            {
                rep.Success = new Success
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = msg.Item1,
                };
                DeviceUtil.LogDeviceCanUse(request.OriginLinuxName, request.OriginIpAddress, request.BusID);
            }
            else
            {
                rep.Error = new Error
                {
                    Code = (int) HttpStatusCode.BadRequest,
                    Message = msg.Item1,
                };
            }

            return Task.FromResult<UsbipCallResp>(rep);
        }
    }
}