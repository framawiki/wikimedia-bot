//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

// Created by Petr Bena benapetr@gmail.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace wmib
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class Configuration
    {
        public class Paths
        {
            /// <summary>
            /// Dump
            /// </summary>
            public static string DumpDir = "dump";
            /// <summary>
            /// This is a log where network log is dumped to
            /// </summary>
            public static string TransactionLog = "transaction.dat";
            public static string ConfigFile = "wmib.conf";
            public static string ChannelFile = "channels.conf";
            public static string GetChannelFile()
            {
                return Variables.ConfigurationDirectory + Path.DirectorySeparatorChar + ChannelFile;
            }
            public static string ModulesPath
            {
                get
                {
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules";
                }
            }
            public static string Security
            {
                get
                {
                    return Variables.ConfigurationDirectory + Path.DirectorySeparatorChar + "security.xml";
                }
            }
        }

        public class WebPages
        {
            /// <summary>
            /// Path to html which is generated by this process
            /// </summary>
            public static string HtmlPath = "html";
            /// <summary>
            /// The webpages url
            /// </summary>
            public static string WebpageURL = "";
            public static string Css = "";
        }

        public class IRC
        {
            /// <summary>
            /// Network the bot is connecting to
            /// </summary>
            public static string NetworkHost = "irc.freenode.net";
            /// <summary>
            /// Nick name
            /// </summary>
            public static string NickName = "wm-bot";
            public static string Ident = "wm-bot";
            /// <summary>
            /// Login name
            /// </summary>
            public static string LoginNick = null;
            /// <summary>
            /// Login pw
            /// </summary>
            public static string LoginPw = null;
            /// <summary>
            /// Whether the bot is using external network module
            /// </summary>
            public static bool UsingBouncer = false;
            /// <summary>
            /// User name
            /// </summary>
            public static string Username = "wm-bot";
            /// <summary>
            /// Interval between messages are sent to server
            /// </summary>
            public static int Interval = 800;
        }

        public class System
        {
            /// <summary>
            /// Uptime
            /// </summary>
            public static DateTime UpTime;
            /// <summary>
            /// Debug channel (doesn't need to exist)
            /// </summary>
            public static string DebugChan = null;
            /// <summary>
            /// Separator for system db
            /// </summary>
            public static string Separator = "|";
            /// <summary>
            /// This is a string which commands are prefixed with
            /// </summary>
            public static string CommandPrefix
            {
                get
                {
                    return prefix;
                }
            }

            public static string prefix = "@";
            /// <summary>
            /// If colors are in terminal
            /// </summary>
            public static bool Colors = true;
            /// <summary>
            /// How verbose the debugging is
            /// </summary>
            public static int SelectedVerbosity = 0;
            /// <summary>
            /// Version
            /// </summary>
            public static string Version = "wikimedia bot v. 2.6.2.0";
            /// <summary>
            /// This is a limit for role level that can be granted, this is used to
            /// prevent users from granting roles like "root" by default
            /// </summary>
            public static int MaxGrantableRoleLevel = 65534;

            /// <summary>
            /// Comma seperated list of modules to load at startup
            /// </summary>
            public static string ModulesToLoad;
            private static string[] _modulesToLoadList;
            public static string[] ModulesToLoadArray
            {
                // we need to replace the newlines so that modules can be stored in config file like:
                // modules=ModuleA,
                //         ModuleB,
                //         ModuleX,
                //         ModuleC;

                get { return _modulesToLoadList ?? (_modulesToLoadList = ModulesToLoad.Replace("\n", "").Split(',')); }
            }
        }

        public class MySQL
        {
            /// <summary>
            /// Mysql user
            /// </summary>
            public static string MysqlUser = null;
            /// <summary>
            /// Mysql pw
            /// </summary>
            public static string MysqlPw = null;
            /// <summary>
            /// Mysql host
            /// </summary>
            public static string MysqlHost = null;
            /// <summary>
            /// Mysql db
            /// </summary>
            public static string Mysqldb = "production";
            /// <summary>
            /// Mysql port
            /// </summary>
            public static int MysqlPort = 3306;
        }

        public class Network
        {
            /// <summary>
            /// Network traffic is logged
            /// </summary>
            public static bool Logging = false;
            /// <summary>
            /// This is a port for default network bouncer
            /// 
            /// This is needed basically for single instance use only
            /// </summary>
            public static int BouncerPort = 6667;
            /// <summary>
            /// This is a port which system console listen on
            /// </summary>
            public static int SystemPort = 2020;
        }

        /// <summary>
        /// List of channels the bot is in, you should never need to use this, use ChannelList instead
        /// </summary>
        public static List<Channel> Channels = new List<Channel>();

        /// <summary>
        /// List of all channels the bot is in, thread safe
        /// </summary>
        public static List<Channel> ChannelList
        {
            get
            {
                List<Channel> list = new List<Channel>();
                lock (Channels)
                {
                    list.AddRange(Channels);
                }
                return list;
            }
        }

        private static Dictionary<string, string> ConfigurationData;

        /// <summary>
        /// Save a wm-bot channel list
        /// </summary>
        public static void Save()
        {
            StringBuilder text = new StringBuilder("");
            lock (Channels)
            {
                foreach (Channel channel in Channels)
                {
                    text.Append(channel.Name + "\n");
                }
            }
            File.WriteAllText(Variables.ConfigurationDirectory + Path.DirectorySeparatorChar + Paths.ChannelFile,
                              text.ToString());
        }

        /// <summary>
        /// Return a temporary name for a file
        /// </summary>
        /// <param name="file">File you need to have temporary name for</param>
        /// <returns></returns>
        public static string TempName(string file)
        {
            return (file + "~");
        }

        public static string RetrieveConfig(string key, string default_ = null)
        {
            if (ConfigurationData.ContainsKey(key))
            {
                return ConfigurationData[key];
            }
            return default_;
        }

        public static int RetrieveConfig(string key, int default_)
        {
            if (ConfigurationData.ContainsKey(key))
            {
                return int.Parse(ConfigurationData[key]);
            }
            return default_;
        }

        private static Dictionary<string, string> File2Dict()
        {
            Dictionary<string, string> Values = new Dictionary<string, string>();
            string[] xx = File.ReadAllLines(Variables.ConfigurationDirectory + Path.DirectorySeparatorChar +
                                            Paths.ConfigFile);
            string LastName = null;
            foreach (string line in xx)
            {
                string content;
                if (String.IsNullOrEmpty(line) || line.TrimStart(' ').StartsWith("//"))
                {
                    continue;
                }
                Syslog.DebugWrite("Parsing line: " + line, 8);
                if (LastName == null && line.Contains("="))
                {
                    LastName = line.Substring(0, line.IndexOf("="));
                    if (Values.ContainsKey(LastName))
                    {
                        throw new Exception("You can't redefine same value in configuration multiple times, error reading: " + LastName);
                    }
                    content = line.Substring(line.IndexOf("=") + 1);
                    if (content.Contains(";"))
                    {
                        content = content.Substring(0, content.IndexOf(";"));
                    }
                    Values.Add(LastName, content);
                    Syslog.DebugWrite("Stored config value: " + LastName + ": " + content);
                    if (line.Contains(";"))
                    {
                        LastName = null;
                    }
                    continue;
                }
                if (LastName != null)
                {
                    // we remove extra space from beginning so that we can indent in config file
                    content = line.TrimStart(' ');
                    if (!content.Contains(";"))
                    {
                        Syslog.DebugWrite("Append config value: " + LastName + ": " + content);
                        Values[LastName] += "\n" + content;
                    }
                    else
                    {
                        content = content.Substring(0, content.IndexOf(";"));
                        Values[LastName] += "\n" + content;
                        Syslog.DebugWrite("Append config value: " + LastName + ": " + content);
                        LastName = null;
                    }
                    continue;
                }
                Syslog.WriteNow("Invalid configuration line: " + line, true);
            }
            return Values;
        }

        /// <summary>
        /// Load config of bot
        /// </summary>
        public static int Load()
        {
            if (Directory.Exists(Variables.ConfigurationDirectory) == false)
            {
                Directory.CreateDirectory(Variables.ConfigurationDirectory);
            }
            if (!File.Exists(Variables.ConfigurationDirectory + Path.DirectorySeparatorChar
                             + Paths.ConfigFile))
            {
                Console.WriteLine("Error: unable to find config file in configuration/"
                    + Paths.ConfigFile);
                Console.WriteLine("You can get a configuration file here: https://github.com/benapetr/wikimedia-bot/blob/master/configuration/wmib.conf");
                return 2;
            }
            ConfigurationData = File2Dict();
            IRC.Username = RetrieveConfig("username");
            IRC.NetworkHost = RetrieveConfig("network");
            IRC.NickName = RetrieveConfig("nick");
            IRC.LoginNick = RetrieveConfig("nick");
            System.DebugChan = RetrieveConfig("debug");
            System.MaxGrantableRoleLevel = RetrieveConfig("maximal_grantable_role_level", System.MaxGrantableRoleLevel);
            System.ModulesToLoad = RetrieveConfig("modules", "");
            Network.BouncerPort = RetrieveConfig("bouncerp", Network.BouncerPort);
            WebPages.WebpageURL = RetrieveConfig("web", "");
            IRC.LoginPw = RetrieveConfig("password", "");
            IRC.Interval = RetrieveConfig("interval", 800);
            MySQL.MysqlPw = RetrieveConfig("mysql_pw");
            MySQL.Mysqldb = RetrieveConfig("mysql_db", MySQL.Mysqldb);
            MySQL.MysqlUser = RetrieveConfig("mysql_user");
            MySQL.MysqlPort = RetrieveConfig("mysql_port", MySQL.MysqlPort);
            MySQL.MysqlHost = RetrieveConfig("mysql_host");
            WebPages.Css = RetrieveConfig("style_html_file", "");
            Network.SystemPort = RetrieveConfig("system_port", Network.SystemPort);
            if (string.IsNullOrEmpty(IRC.LoginNick))
            {
                Console.WriteLine("Error there is no login for bot (nick key is missing?)");
                return 1;
            }
            if (string.IsNullOrEmpty(IRC.NetworkHost))
            {
                Console.WriteLine("Error irc server is wrong (network key is missing?)");
                return 4;
            }
            if (string.IsNullOrEmpty(IRC.NickName))
            {
                Console.WriteLine("Error there is no username for bot");
                return 6;
            }
            System.prefix = RetrieveConfig("system_prefix", System.prefix);
            IRC.UsingBouncer = bool.Parse(RetrieveConfig("serverIO", "false"));
            Syslog.Log("Loading instances");
            Instance.PrimaryInstance = Instance.CreateInstance(IRC.NickName, Network.BouncerPort);
            int CurrentInstance = 0;
            while (CurrentInstance < 20)
            {
                if (!ConfigurationData.ContainsKey("instancename" + CurrentInstance))
                {
                    break;
                }
                string InstanceName = ConfigurationData["instancename" + CurrentInstance];
                Syslog.DebugLog("Instance found: " + InstanceName);
                if (IRC.UsingBouncer)
                {
                    Syslog.DebugLog("Using bouncer, looking for instance port");
                    if (!ConfigurationData.ContainsKey("instanceport" + CurrentInstance))
                    {
                        Syslog.WriteNow("Instance " + InstanceName + " has invalid port, not using", true);
                        continue;
                    }
                    int port = int.Parse(ConfigurationData["instanceport" + CurrentInstance]);
                    Instance.CreateInstance(InstanceName, port);
                }
                else
                {
                    Instance.CreateInstance(InstanceName);
                }
                CurrentInstance++;
            }
            if (!File.Exists(Paths.GetChannelFile()))
            {
                Console.WriteLine("Error there is no channel file (" + Paths.GetChannelFile() + ") to load channels from");
                return 20;
            }
            foreach (string x in File.ReadAllLines(Paths.GetChannelFile()))
            {
                string name = x.Replace(" ", "");
                if (!string.IsNullOrEmpty(name))
                {
                    lock (Channels)
                    {
                        Channels.Add(new Channel(name));
                    }
                }
            }
            Syslog.Log("Channels were all loaded, linking databases");

            // Now when all chans are loaded let's link them together
            lock (Channels)
            {
                foreach (Channel channel in Channels)
                {
                    channel.InitializeShares();
                }
            }

            Syslog.Log("Channel db's working");

            if (!Directory.Exists(Paths.DumpDir))
            {
                Directory.CreateDirectory(Paths.DumpDir);
            }
            return 0;
        }
    }
}
