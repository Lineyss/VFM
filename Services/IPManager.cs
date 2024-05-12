using Microsoft.OpenApi.Extensions;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using Microsoft.Extensions.Configuration.Json;
using VFM.Models;

using Newtonsoft.Json;

namespace VFM.Services
{
    public class IPManager
    {
        public static IPAddress[] getAddress()
        {
            string host = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(host);

            IPAddress[] addresses = hostEntry.AddressList
                                        .Where(addr => addr.AddressFamily == AddressFamily.InterNetwork)
                                        .ToArray();

            return addresses;
        }
    }
}
