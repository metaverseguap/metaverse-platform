using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkCore.Utils
{
    /// <summary>
    /// <para>Вспомогательные методы для работы с IP.</para>
    /// </summary>
    public static class IPUtils
    {
        /// <summary>
        /// <para>Получить ip адрес машины в локальной сети.</para>
        /// </summary>
        /// <returns>ip адрес машины в локальной сети</returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new InvalidOperationException("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// <para>Получить Mirror Url используя ip машины в локальной сети.</para>
        /// </summary>
        /// <returns>Mirror Url</returns>
        public static string GetIpAsUrl()
        {
            string ip = GetLocalIPAddress();
            return UrlFromIP(ip);
        }

        /// <summary>
        /// <para>Преобразовать ip к Mirror url.</para>
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>Mirror url</returns>
        public static string UrlFromIP(string ip)
        {
            return $"kcp://{ip}";
        }
    }
}