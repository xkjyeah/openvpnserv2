using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
            this.AutoLog = true;

            this.Subprocesses = new List<OpenVpnChild>();
        }
        public static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new OpenVpnService());
        }

        protected override void OnStop()
        {
            foreach (var child in Subprocesses)
            {
                child.StopProcess();
            }
            
            foreach (var child in Subprocesses)
            {
                child.Wait();
            }
        }

        protected override void OnStart(string[] args)
        {
            RegistryKey rkHKCU = Registry.LocalMachine;

            try
            {
                var rkOvpn = rkHKCU.OpenSubKey("Software\\OpenVPN", false);

                if (rkOvpn == null)
                    throw new Exception("Registry key missing");

                bool append = false;
                {
                    var logAppend = (string)rkOvpn.GetValue("log_append");
                    if (logAppend[0] == '0' || logAppend[0] == '1')
                        append = logAppend[0] == '1';
                    else
                        throw new Exception("Log file append flag must be 1 or 0");
                }
                    
                var config = new OpenVpnServiceConfiguration() {
                    exePath = (string)rkOvpn.GetValue("exe_path"),
                    configDir = (string)rkOvpn.GetValue("config_dir"),
                    configExt = "." + (string)rkOvpn.GetValue("config_ext"),
                    logDir = (string)rkOvpn.GetValue("log_dir"),
                    logAppend = append,
                    priorityClass = GetPriorityClass((string)rkOvpn.GetValue("priority")),
                    
                    eventLog = EventLog,
                };

                foreach (string _filename in Directory.GetFiles(config.configDir))
                {
                    if (!_filename.EndsWith(config.configExt))
                    {
                        continue;
                    }
                    
                    Subprocesses.Add(new OpenVpnChild(config, _filename));
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
            
            ReinitProcess();
        }
        
        public void StopProcess() {
            if (restartTimer != null) {
                restartTimer.Stop();
            }
            if (!process.HasExited) {
                process.EnableRaisingEvents = false;
                process.CloseMainWindow();
                
                var stopTimer = new System.Timers.Timer(3000);
                stopTimer.AutoReset = false;
                stopTimer.Elapsed += (object source, System.Timers.ElapsedEventArgs e) =>
                    {
                        process.Kill();
                    };
                stopTimer.Start();
            }
        }
        
        public void Wait() {
            process.WaitForExit();
            logFile.Close();
        }
        
        private void ReinitProcess() {
            process = new System.Diagnostics.Process();

            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
            {
                if (e != null)
                    logFile.WriteLine(e.Data);
            };

            process.ErrorDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
            {
                if (e != null)
                    logFile.WriteLine(e.Data);
            };

            process.Exited += (object sender, EventArgs e) =>
            {
                config.eventLog.WriteEntry("Process for " + configFile + " exited. Restarting in 10 sec.");

                restartTimer = new System.Timers.Timer(10000);
                restartTimer.AutoReset = false;
                restartTimer.Elapsed += (object source, System.Timers.ElapsedEventArgs ev) =>
                    {
                        ReinitProcess();
                    };
                restartTimer.Start();
            };

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.PriorityClass = config.priorityClass;        
        }
    
    }
}
