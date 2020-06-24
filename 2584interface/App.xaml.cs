using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace _2584interface
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string game = ConfigurationManager.AppSettings["game"] ?? "2048";
        public static string aiServerIp = ConfigurationManager.AppSettings["ai_server_ip"] ?? string.Empty;
        public static string aiServerPort = ConfigurationManager.AppSettings["ai_server_port"] ?? string.Empty;
        public static string start_time = ConfigurationManager.AppSettings["start_time"] ?? "00:00:01";
        public static string end_time = ConfigurationManager.AppSettings["end_time"] ?? "07:00:01";
    }
}
