using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetworkCore.Utils
{
    /// <summary>
    /// <para>Вспомогательные методы для работы с IP.</para>
    /// </summary>
    public static class IPUtils
    {
        /// <summary>
        /// <para>Порт по умолчанию для запуска offline сцены.</para>
        /// </summary>
        public const int DEFAULT_PORT_FOR_OFFLINE = 10001;
        
        private const int MAX_AVAILABLE_PORT = 65535;
        private const int MIN_AVAILABLE_PORT = 7777;
        
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
            string urlPrefix = UrlPrefix();
            return $"{urlPrefix}{ip}";
        }

        /// <summary>
        /// <para>Получить URL префикс подключения к сети Mirror.</para>
        /// </summary>
        /// <returns>URL префикс подключения к сети Mirror</returns>
        public static string UrlPrefix()
        {
            return "kcp://";
        }

        /// <summary>
        /// <para>Проверяет, является ли URL валидным.</para>
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>true, если url является валидным</returns>
        public static bool IsURLValid(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                return !string.IsNullOrEmpty(uriResult.Scheme);
            }
            
            return false;
        }

        /// <summary>
        /// <para>Получает доступный порт из указанного количества.</para>
        /// Доступные порты получаются из портов больших или меньших <see cref="DEFAULT_PORT_FOR_OFFLINE"/> 
        /// </summary>
        /// <param name="range">количество портов</param>
        /// <returns>свободный порт или null, если все порты заняты</returns>
        public static int? GetAvailablePortUDP(int range)
        {
            if ((range <= 0 || range > MAX_AVAILABLE_PORT)
                || ((DEFAULT_PORT_FOR_OFFLINE + range >= MAX_AVAILABLE_PORT) || (DEFAULT_PORT_FOR_OFFLINE - range <= MIN_AVAILABLE_PORT)))
            {
                return null;
            }

            if (DEFAULT_PORT_FOR_OFFLINE + range <= MAX_AVAILABLE_PORT)
            {
                return GetAvailablePortUDP(DEFAULT_PORT_FOR_OFFLINE, DEFAULT_PORT_FOR_OFFLINE + range);
            }

            return GetAvailablePortUDP(DEFAULT_PORT_FOR_OFFLINE - range, DEFAULT_PORT_FOR_OFFLINE);
        }
        
        /// <summary>
        /// <para>Получает доступный порт в указанном диапазоне.</para>
        /// </summary>
        /// <param name="from">порт начала диапазона</param>
        /// <param name="to">порт конца диапазона</param>
        /// <returns>свободный порт из указанного диапазона или null, если все порты в диапазоне заняты</returns>
        public static int? GetAvailablePortUDP(int from, int to)
        {
            ISet<int> occupiedPorts =
                IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveUdpListeners()
                    .Select(p => p.Port)
                    // .OrderBy(p => p) // В debug проще смотреть на отсортированные значения 
                    .ToHashSet();

            for (int port = from; port <= to; port++)
            {
                if (!occupiedPorts.Contains(port))
                {
                    return port;
                }
            }

            return null;
        }
    }
}