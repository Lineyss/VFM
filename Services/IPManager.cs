using Microsoft.OpenApi.Extensions;
using System.Net;
using System.Net.Sockets;

namespace VFM.Services
{
    public class IPManager
    {
        public static string getAddress()
        {
            string host = Dns.GetHostName();
            var list = Dns.GetHostByName(host).AddressList;

            string? ip = list.FirstOrDefault(element => element.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            return ip ?? "localhost";
        }
    }
}
