using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace OpenVpn
{
    [ServiceContract]
    public interface IInteractiveService
    {
        [OperationContract]
        string[] EnumerateConfigs();

        [OperationContract]
        string[] EnumerateRunningServices();

        /// <summary>
        /// Start the VPN specified by this configuration file
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns>false if the service has already been started, or the
        /// configuratin is invalid. true if a child process has been started</returns>
        [OperationContract]
        bool StartVPN(string configFile);

        /// <summary>
        /// Stop the VPN specified by this configuration file.
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns>true if a running configuration exists and the 
        /// stop command was sent. False otherwise</returns>
        [OperationContract]
        bool StopVPN(string configFile);
    }
}
