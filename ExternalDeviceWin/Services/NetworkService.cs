using Grpc.Core;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ExternalDeviceWin.Services
{
    public class NetworkService: NetworkConnecter.NetworkConnecterBase
    {
        private static string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = String.Empty;
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
            if(!Enum.IsDefined(typeof(NetworkInterfaceType), request.NetworkType))
            {
                return Task.FromResult(new AddressResp
                {
                    Ipv4Add = String.Empty,
                    Error = new Error
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "your network type is wrong",
                    }
                });
            }

            return Task.FromResult(new AddressResp
            {
                Ipv4Add = GetLocalIPv4((NetworkInterfaceType)request.NetworkType),
                Success = new Success
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "OK",
                }
            });
        }
    }
}
