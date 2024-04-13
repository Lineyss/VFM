using Microsoft.OpenApi.Extensions;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace VFM.Services
{
    public class IPManager
    {
        public static IPAddress[] getAddress()
        {
            string host = Dns.GetHostName();
            var list = Dns.GetHostByName(host).AddressList;

            IPAddress[] ips = list.Where(element => element.AddressFamily == AddressFamily.InterNetwork).ToArray();
            return ips;
        }
    }
}
