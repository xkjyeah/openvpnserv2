using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;

namespace OpenVpn
{
    class OpenVpnService : System.ServiceProcess.ServiceBase
    {
        public const string Package = "openvpn";
        private List<OpenVpnChild> Subprocesses;

        public OpenVpnService()
        {
            this.ServiceName = "OpenVpnService";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.CanHandlePowerEvent = false; // N.B. if OpenVPN always dies when suspending, then this is unnecessary
            this.AutoLog = true;

            this.Subprocesses = new List<OpenVpnChild>();
        }
        public static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new OpenVpnService());
        }

        protected override void OnStop()
        {
            RequestAdditionalTime(3000);
            foreach (var child in Subprocesses)
            {
                child.StopProcess();
            }
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            if (powerStatus == PowerBroadcastStatus.Suspend)
            {
                //EventLog.WriteEntry("Computer going to sleep");
                foreach (var child in Subprocesses)
                {
                    child.StopProcess();
                }
            }
            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeCritical))
            {
                //EventLog.WriteEntry("Resume from critical");
                foreach (var child in Subprocesses)
                {
                    child.Restart();
                }
            }
            else if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeSuspend))
            {
                //EventLog.WriteEntry("Resume from suspend");
                foreach (var child in Subprocesses)
                {
                    child.Start();
                }
            }
            return true;
        }

        protected override void OnStart(string[] args)
        {
            RegistryKey rkHKLM = Registry.LocalMachine;
            List<RegistryKey> rkOvpns = new List<RegistryKey>();

            EventLog.WriteEntry("Start (1)");
            try
            {
                var key = rkHKLM.OpenSubKey("Software\\OpenVPN", false);
                if (key != null) rkOvpns.Add(key);
                key = rkHKLM.OpenSubKey("Software\\Wow6432Node\\OpenVPN", false);
                if (key != null) rkOvpns.Add(key);

                if (rkOvpns.Count() == 0)
                    throw new Exception("Registry key missing");

                foreach (var rkOvpn in rkOvpns)
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

                        eventLog = EventLog,
                    };

                    EventLog.WriteEntry("Start (2) Config dir: " + config.configDir);

                    /// Only attempt to start the service
                    /// if openvpn.exe is present. This should help if there are old files
                    /// and registry settings left behind from a previous OpenVPN 32-bit installation
                    /// on a 64-bit system.
                    if (!File.Exists(config.exePath))
                    {
                        EventLog.WriteEntry("OpenVPN binary does not exist at " + config.exePath);
                        continue;
                    }

                    foreach (string _filename in Directory.GetFiles(config.configDir))
                    {
                        EventLog.WriteEntry("Start (3) Found file: " + _filename);
                        if (!_filename.EndsWith(config.configExt))
                        {
                            continue;
                        }
                        EventLog.WriteEntry("Start (4) Processing config file: " + _filename);

                        var child = new OpenVpnChild(config, _filename);
                        Subprocesses.Add(child);
                        child.Start();
                    }
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Exception occured during service start: " + e.Message);
                throw e;
            }
        }

        private System.Diagnostics.ProcessPriorityClass GetPriorityClass(string priorityString)
        {
            if (String.Equals(priorityString, "IDLE_PRIORITY_CLASS", StringComparison.InvariantCultureIgnoreCase)) {
                return System.Diagnostics.ProcessPriorityClass.Idle;
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

        private void InitializeComponent()
        {
            // 
            // OpenVpnService
            // 
            this.ServiceName = "OpenVpnService";

        }
    }
    
    class OpenVpnServiceConfiguration {
        public string exePath {get;set;}
        public string configExt {get;set;}
        public string configDir {get;set;}
        public string logDir {get;set;}
        public bool logAppend {get;set;}
        public System.Diagnostics.ProcessPriorityClass priorityClass {get;set;}
        
        public EventLog eventLog {get;set;}
    }
    
    class OpenVpnChild {
        StreamWriter logFile;
        Process process;
        ProcessStartInfo startInfo;
        System.Timers.Timer restartTimer;
        OpenVpnServiceConfiguration config;
        string configFile;
    
        public OpenVpnChild(OpenVpnServiceConfiguration config, string configFile) {
            this.config = config;
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
                FileMode.OpenOrCreate | (config.logAppend ? FileMode.Append : FileMode.Truncate),
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
        
        public void StopProcess() {
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
}
