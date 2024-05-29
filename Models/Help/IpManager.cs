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
                    string url = $"http://{element}:80";
                    _urls.Add(url);
                    Console.WriteLine($"{url}/Auth/Login.html");
                    Console.WriteLine($"{url}/VirtualFileManager");
                    Console.WriteLine("----------------------");
                }
            }

            urls = _urls.ToArray();
        }
    }
}
