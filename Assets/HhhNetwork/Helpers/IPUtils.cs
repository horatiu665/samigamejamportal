namespace HhhNetwork
{
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public static class IPUtils
    {
        public static string GetLocalIPv4(NetworkInterfaceType type)
        {
            string output = string.Empty;
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                var item = interfaces[i];
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    var addresses = item.GetIPProperties().UnicastAddresses;
                    var count = addresses.Count;
                    for (int j = 0; j < count; j++)
                    {
                        var ip = addresses[j];
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }

            return output;
        }
    }
}