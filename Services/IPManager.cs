using Microsoft.OpenApi.Extensions;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace VFM.Services
{
    public class IPManager
    {
        public static string[] getAddress()
        {
            List<string> _ips = new List<string>();

            string host = Dns.GetHostName();
            var list = Dns.GetHostByName(host).AddressList;

            IPAddress[] ips = list.Where(element => element.AddressFamily == AddressFamily.InterNetwork).ToArray();
            foreach(var ip in ips)
            {
                _ips.Add($"http://{ip.ToString()}:5000");
            }
            return _ips.ToArray();
        }
    }
}
