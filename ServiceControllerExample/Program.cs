using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace OpenVpn
{
    class Program
    {
        static void Main(string[] args)
        {
            var binding = new NetNamedPipeBinding();
            var endpointAddress = new EndpointAddress(OpenVpnServiceInfo.EndpointAddress);
            var channelFactory = new ChannelFactory<IInteractiveService>(binding, endpointAddress);

            IInteractiveService ovpnService = null;
            try
            {
                ovpnService = channelFactory.CreateChannel();
                if (args[0].Equals("list", StringComparison.CurrentCultureIgnoreCase)) {
                    Console.WriteLine("Available configuration files:");
                    foreach (var s in ovpnService.EnumerateConfigs())
                    {
                        Console.WriteLine(s);
                    }

                    Console.WriteLine("Running configurations:");
                    foreach (var s in ovpnService.EnumerateRunningServices())
                    {
                        Console.WriteLine(s);
                    }
                }
                else if (args[0].Equals("stop", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(ovpnService.StopVPN(args[1]));
                }
                else if (args[0].Equals("start", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(ovpnService.StartVPN(args[1]));
                }
                else
                {
                    throw new Exception("Unknown instruction");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message + e.InnerException.StackTrace);
                }
            }
        }
    }
}
