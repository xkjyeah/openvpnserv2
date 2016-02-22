using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;

namespace OpenVpn
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                    ConcurrencyMode = ConcurrencyMode.Single,
                    IncludeExceptionDetailInFaults=true)]
    public class OpenVpnService : ServiceBase, IInteractiveService
    {
        public const string Package = "openvpn";
        private Dictionary<string, OpenVpnServiceConfiguration> configurations;
        private Dictionary<string, OpenVpnChild> Subprocesses;
        private System.ServiceModel.ServiceHost serviceHost;

        public OpenVpnService()
        {
            this.ServiceName = "OpenVpnService";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            // N.B. if OpenVPN always dies when suspending, then this is unnecessary
            // However if there is some kind of stuck state where OpenVPN.exe hangs
            // after resuming, then this will help
            this.CanHandlePowerEvent = false; 
            this.AutoLog = true;

            this.Subprocesses = new Dictionary<string, OpenVpnChild>();
            this.configurations = new Dictionary<string, OpenVpnServiceConfiguration>();
        }

        public static void Main()
        {
            var service = new OpenVpnService();
            ServiceBase.Run(service);
        }

        protected override void OnStop()
        {
            RequestAdditionalTime(3000);
            foreach (var child in Subprocesses)
            {
                child.Value.Stop();
            }
            serviceHost.Close();
        }

        /// <summary>
        /// Forcibly stops the process on suspend, and starts it again on resume.
        /// </summary>
        /// <param name="powerStatus"></param>
        /// <returns></returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            if (powerStatus == PowerBroadcastStatus.Suspend)
            {
                //EventLog.WriteEntry("Computer going to sleep");
                foreach (var child in Subprocesses)
                {
                    child.Value.Stop();
                }
            }
            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeCritical))
            {
                //EventLog.WriteEntry("Resume from critical");
                foreach (var child in Subprocesses)
                {
                    child.Value.Restart();
                }
            }
            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeSuspend))
            {
                //EventLog.WriteEntry("Resume from suspend");
                foreach (var child in Subprocesses)
                {
                    child.Value.Start();
                }
            }
            return true;
        }

        private RegistryKey GetRegistrySubkey(RegistryView rView)
        {
            try
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, rView)
                    .OpenSubKey("Software\\OpenVPN");
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        private void PrepareInteractiveService()
        {
            // Bind the named pipe
            this.serviceHost = new System.ServiceModel.ServiceHost(this, new Uri(OpenVpnServiceInfo.EndpointAddress));
        }

        protected override void OnStart(string[] args)
        {
            // Start the services
            EnumerateConfigs();

            foreach (var fileConfigPair in configurations)
            {
                // file has been marked for manual startup, so don't start automatically
                if (fileConfigPair.Value.manualStartup.Contains(System.IO.Path.GetFileName(fileConfigPair.Key)))
                {
                    continue;
                }

                try {
                    var child = new OpenVpnChild(fileConfigPair.Value, fileConfigPair.Key);
                    Subprocesses.Add(fileConfigPair.Key, child);
                    child.Start();
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("Error starting config " + fileConfigPair.Key + ": " + e);
                }
            }

            /* Try to start the service, but because it's not crucial don't let it kill the service process */
            try {
                PrepareInteractiveService();
                serviceHost.Open();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Error starting config " + e.Message + e.StackTrace);
            }
        }

        private ProcessPriorityClass GetPriorityClass(string priorityString)
        {
            if (priorityString.Equals("IDLE_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase)) {
                return ProcessPriorityClass.Idle;
            }
            else if (String.Equals(priorityString, "BELOW_NORMAL_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase))
            {
                return System.Diagnostics.ProcessPriorityClass.BelowNormal;
            }
            else if (String.Equals(priorityString, "NORMAL_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase))
            {
                return System.Diagnostics.ProcessPriorityClass.Normal;
            }
            else if (String.Equals(priorityString, "ABOVE_NORMAL_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase))
            {
                return System.Diagnostics.ProcessPriorityClass.AboveNormal;
            }
            else if (String.Equals(priorityString, "HIGH_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase))
            {
                return System.Diagnostics.ProcessPriorityClass.High;
            }
            else {
                throw new Exception("Unknown priority name: " + priorityString);
            }
        }

        public string[] EnumerateConfigs()
        {
            configurations.Clear();

            List<RegistryKey> rkOvpns = new List<RegistryKey>();
            var key = GetRegistrySubkey(RegistryView.Registry64);
            Console.WriteLine(key == null);
            if (key != null) rkOvpns.Add(key);
            key = GetRegistrySubkey(RegistryView.Registry32);
            Console.WriteLine(key == null);
            if (key != null) rkOvpns.Add(key);

            if (rkOvpns.Count() == 0)
                throw new Exception("Registry key missing");

            foreach (var rkOvpn in rkOvpns)
            {
                try
                {
                    bool append = false;
                    {
                        var logAppend = (string)rkOvpn.GetValue("log_append");
                        if (logAppend[0] == '0' || logAppend[0] == '1')
                            append = logAppend[0] == '1';
                        else
                            throw new Exception("Log file append flag must be 1 or 0");
                    }

                    var config = new OpenVpnServiceConfiguration()
                    {
                        exePath = (string)rkOvpn.GetValue("exe_path"),
                        configDir = (string)rkOvpn.GetValue("config_dir"),
                        configExt = "." + (string)rkOvpn.GetValue("config_ext"),
                        logDir = (string)rkOvpn.GetValue("log_dir"),
                        logAppend = append,
                        priorityClass = GetPriorityClass((string)rkOvpn.GetValue("priority")),

                        manualStartup = ((string)rkOvpn.GetValue("manual_startup") ?? "")
                                    .Split(new string [] { ";" }, StringSplitOptions.RemoveEmptyEntries),
                        
                        userControl = ((string)rkOvpn.GetValue("user_control") ?? "")
                                    .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries),

                        eventLog = EventLog,
                    };

                    /// Only attempt to start the service
                    /// if openvpn.exe is present. This should help if there are old files
                    /// and registry settings left behind from a previous OpenVPN 32-bit installation
                    /// on a 64-bit system.
                    if (!File.Exists(config.exePath))
                    {
                        EventLog.WriteEntry("OpenVPN binary does not exist at " + config.exePath);
                        continue;
                    }

                    foreach (var configFilename in Directory.EnumerateFiles(config.configDir,
                                                                            "*" + config.configExt,
                                                                            System.IO.SearchOption.AllDirectories))
                    {
                        try
                        {
                            configurations.Add(configFilename, config);
                        }
                        catch (Exception e)
                        {
                            EventLog.WriteEntry("Caught exception " + e.Message + " when starting openvpn for "
                                + configFilename);
                        }
                    }
                }
                catch (NullReferenceException e) /* e.g. missing registry values */
                {
                    EventLog.WriteEntry("Registry values are incomplete for " + rkOvpn.View.ToString() + e.StackTrace);
                }
            }
            return configurations.Keys.ToArray();
        }
        
        public bool StartVPN(string configFile)
        {
            // Start the service
            if (configurations.ContainsKey(configFile))
            {
                if (!IsCalledAsAdministrator() &&
                    !configurations[configFile].userControl.Contains(System.IO.Path.GetFileName(configFile)))
                {
                    throw new System.Security.SecurityException("User is not allowed to control " + configFile);
                }

                // Ensure the service isn't already running
                if (Subprocesses.ContainsKey(configFile))
                {
                    throw new InvalidOperationException("Service " + configFile + " already running");
                }

                var child = new OpenVpnChild(configurations[configFile], configFile);
                Subprocesses.Add(configFile, child);
                child.Start();
                return true;
            }
            return false;
        }

        public bool StopVPN(string configFile)
        {
            if (Subprocesses.ContainsKey(configFile))
            {
                if (!IsCalledAsAdministrator() &&
                    !Subprocesses[configFile].config.userControl.Contains(System.IO.Path.GetFileName(configFile)))
                {
                    throw new System.Security.SecurityException("User is not allowed to control " + configFile);
                }

                Subprocesses[configFile].Stop();
                Subprocesses[configFile].Dispose();
                Subprocesses.Remove(configFile);
                return true;
            }
            return false;
        }

        public string[] EnumerateRunningServices()
        {
            return Subprocesses.Keys.ToArray();
        }

        private bool IsCalledAsAdministrator()
        {
            return new System.Security.Principal.WindowsPrincipal(ServiceSecurityContext.Current.WindowsIdentity)
                .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
    
    class OpenVpnServiceConfiguration {
        public string exePath {get;set;}
        public string configExt {get;set;}
        public string configDir {get;set;}
        public string logDir {get;set;}
        public bool logAppend {get;set;}
        /// <summary>
        /// Whether normal users are allowed to start/stop this service
        /// </summary>
        public string[] manualStartup { get; set; }
        public string[] userControl { get; set; }
        public ProcessPriorityClass priorityClass {get;set;}
        
        public EventLog eventLog {get;set;}
    }
    
    class OpenVpnChild {
        StreamWriter logFile;
        Process process;
        ProcessStartInfo startInfo;
        System.Timers.Timer restartTimer;
        private OpenVpnServiceConfiguration _config;
        string configFile;
        
        public OpenVpnServiceConfiguration config { get { return _config; } }

        public OpenVpnChild(OpenVpnServiceConfiguration config, string configFile) {
            this._config = config;
            /// SET UP LOG FILES
            /* Because we will be using the filenames in our closures,
             * so make sure we are working on a copy */
            this.configFile = String.Copy(configFile);
            var justFilename = System.IO.Path.GetFileName(configFile);
            var logFilename = config.logDir + "\\" +
                    justFilename.Substring(0, justFilename.Length - config.configExt.Length) + ".log";
            
            // FIXME: if (!init_security_attributes_allow_all (&sa))
            //{
            //    MSG (M_SYSERR, "InitializeSecurityDescriptor start_" PACKAGE " failed");
            //    goto finish;
            //}
            
            logFile = new StreamWriter(File.Open(logFilename,
                config.logAppend ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.Read), new UTF8Encoding(false));
            
            /// SET UP PROCESS START INFO
            string[] procArgs = {
                "--config",
                "\"" + configFile + "\""
            };
            this.startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,

                FileName = config.exePath,
                Arguments = String.Join(" ", procArgs),
                WorkingDirectory = config.configDir,

                UseShellExecute = false,
                /* create_new_console is not exposed -- but we probably don't need it?*/
            };
            
            /// SET UP FLUSH TIMER
            /** .NET has a very annoying habit of taking a very long time to flush
                output streams **/
            var flushTimer = new System.Timers.Timer(60000);
            flushTimer.AutoReset = true;
            flushTimer.Elapsed += (object source, System.Timers.ElapsedEventArgs e) =>
                {
                    logFile.Flush();
                };
            flushTimer.Start();
        }
        
        public void Stop() {
            if (restartTimer != null) {
                restartTimer.Stop();
            }
            try
            {
                if (!process.HasExited)
                {
                    process.Exited -= Watchdog; // Don't restart the process after kill
                    process.Kill();
                }
            }
            catch (InvalidOperationException) { }
        }

        public void Dispose()
        {
            this.logFile.Dispose();
        }
        
        public void Wait() {
            process.WaitForExit();
            logFile.Close();
        }

        public void Restart() {
            if (restartTimer != null) {
                restartTimer.Stop();
            }
            /* try-catch... because there could be a concurrency issue (write-after-read) here? */
            if (!process.HasExited)
            {
                process.Exited -= Watchdog;
                process.Exited += FastRestart; // Restart the process after kill
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                    Start();
                }
            }
            else
            {
                Start();
            }
        }

        private void WriteToLog(object sendingProcess, DataReceivedEventArgs e) {
            if (e != null)
                logFile.WriteLine(e.Data);
        }

        /// Restart after 10 seconds
        /// For use with unexpected terminations
        private void Watchdog(object sender, EventArgs e)
        {
            config.eventLog.WriteEntry("Process for " + configFile + " exited. Restarting in 10 sec.");

            restartTimer = new System.Timers.Timer(10000);
            restartTimer.AutoReset = false;
            restartTimer.Elapsed += (object source, System.Timers.ElapsedEventArgs ev) =>
                {
                    Start();
                };
            restartTimer.Start();
        }

        /// Restart after 3 seconds
        /// For use with Restart() (e.g. after a resume)
        private void FastRestart(object sender, EventArgs e)
        {
            config.eventLog.WriteEntry("Process for " + configFile + " restarting in 3 sec");
            restartTimer = new System.Timers.Timer(3000);
            restartTimer.AutoReset = false;
            restartTimer.Elapsed += (object source, System.Timers.ElapsedEventArgs ev) =>
                {
                    Start();
                };
            restartTimer.Start();
        }
        
        public void Start() {
            process = new System.Diagnostics.Process();

            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += WriteToLog;
            process.ErrorDataReceived += WriteToLog;
            process.Exited += Watchdog;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.PriorityClass = config.priorityClass;        
        }   
    }

    /// <summary>
    /// Helper class to allow service referers to find the endpoint
    /// </summary>
    public class OpenVpnServiceInfo
    {
        public const string EndpointAddress = "net.pipe://localhost/openvpn/InteractiveService";
    }

}
