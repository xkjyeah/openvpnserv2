using System;
using NUnit.Framework;
using OpenVpn;
using Urasandesu.Prig.Framework;
using System.Diagnostics.Prig;
using System.Diagnostics;

namespace OpenVpnServiceTest
{
    [TestFixture]
    public class BasicTests
    {
        private bool AssertHasOption(string[] args, string option, string expected, bool quoted = false)
        {
            for (int i=0; i<args.Length - 1; i++)
            {
                if (args[i] == option)
                {
                    if (quoted)
                    {
                        Assert.AreEqual(args[i + 1], "\"" + expected + "\"");
                        return true;
                    }
                    else
                    {
                        Assert.AreEqual(args[i + 1], expected);
                        return true;
                    }
                }
            }
            Assert.Fail("Option " + option + " was not found");
            return false;
        }

        [Test]
        public void AlwaysPasses()
        {
            using (new IndirectionsContext())
            {
                PProcess.StartString().Body = (string s) =>
                {
                    Assert.AreEqual("Hey", s);
                    return null;
                };

                Process.Start("Hey");
            }
        }

        [Test]
        public void TestLaunchArguments1()
        {
            using (new IndirectionsContext())
            {
                var config = new OpenVpnServiceConfiguration();

                config.exePath = "C:\\Windows\\notexists.exe";
                config.configExt = ".ovpn";
                config.configDir = "C:\\Windows\\Temp";
                config.logAppend = true;
                config.logDir = System.IO.Path.GetTempPath();
                config.priorityClass = ProcessPriorityClass.BelowNormal;

                var configFile = System.IO.Path.GetRandomFileName();
                var numExecuted = 0;

                OpenVpnChild child = new OpenVpnChild(config, configFile);

                PProcess.BeginErrorReadLine().Body = (Process p) => { };
                PProcess.BeginOutputReadLine().Body = (Process p) => { };
                PProcess.PriorityClassSetProcessPriorityClass().Body = (Process p, ProcessPriorityClass c) =>
                {
                    numExecuted++;
                    Assert.AreEqual(c, config.priorityClass);
                };

                PProcess.Start().Body = (Process process) =>
                {
                    numExecuted++;

                    Assert.AreEqual(process.StartInfo.FileName, config.exePath);
                    Assert.AreEqual(process.StartInfo.WorkingDirectory, config.configDir);
                    // Split up the arguments
                    char[] sepChars = new char[] { ' ' };
                    string[] arguments = process.StartInfo.Arguments.Split(
                        sepChars,
                        StringSplitOptions.RemoveEmptyEntries);

                    AssertHasOption(arguments, "--config", configFile, true);
                    // AssertHasOption(arguments, "--log-append", config.logDir + "\\" + configFile, true));

                    return true;
                };

                child.Start();
                Assert.AreEqual(numExecuted, 2);
            }
        }
    }
}
