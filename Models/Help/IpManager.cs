using System.Net;
using System.Net.Sockets;

namespace VFM.Models.Help
{
    public static class IpManager
    {
        public static string[] urls;

        static IpManager()
        {
            List<string> _urls = new List<string>();
            string host = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(host);

            foreach (IPAddress element in hostEntry.AddressList)
            {
                if (element.AddressFamily == AddressFamily.InterNetwork)
                {
                    _urls.Add($"http://{element}:80");
                    _urls.Add($"https://{element}:443");
                }
            }

            urls = _urls.ToArray();
        }
    }
}
