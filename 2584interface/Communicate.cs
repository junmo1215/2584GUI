using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _2584interface
{
    class Communicate
    {
        private static long ip;
        private static int port;

        public static void Config()
        {
            ip = ParseIp(App.aiServerIp);
            port = int.Parse(App.aiServerPort);
        }

        public static string SendMessage(string message)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(message);
            Byte[] bytesReceived = new Byte[256];

            // Create a socket connection with the specified server and port.
            Socket s = ConnectSocket();

            if (s == null)
                return ("Connection failed");

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            // Receive the server home page content.
            int bytes = 0;
            string page = string.Empty;

            // The following will block until the page is transmitted.
            do
            {
                try
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                catch (Exception)
                {
                    bytes = -1;
                }

            }
            while (bytes > 0);

            return page;
        }

        /// <summary>
        /// 点分十进制ipv4地址转换成long型
        /// </summary>
        /// <param name="strIp"></param>
        /// <returns></returns>
        private static long ParseIp(string strIp)
        {
            byte[] address = IPAddress.Parse(strIp).GetAddressBytes();
            long m_Address = ((address[3] << 24 | address[2] << 16 | address[1] << 8 | address[0]) & 0xFFFFFFFF);
            return m_Address;
        }

        private static Socket ConnectSocket()
        {
            IPAddress address = new IPAddress(ip);
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            s.Connect(ipe);

            if (s.Connected)
            {
                return s;
            }
            else
            {
                return null;
            }
        }
    }
}
