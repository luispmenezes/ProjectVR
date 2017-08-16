using System;
using System.IO;
using System.Net;
using System.Net.Sockets; 

namespace Microsoft.Samples.Kinect.DepthBasics{
    
    static class Transmission
    {
        static Socket listener,handler;
        static StreamWriter tw = new StreamWriter("networkLog.txt");

        public static bool startServer(String IP, int port) {
            IPAddress ipAddress = IPAddress.Parse(IP);
            IPEndPoint localEP = new IPEndPoint(ipAddress, port);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            tw.WriteLine("Server Created: IP: " + IP + " Port: " + port);
            tw.WriteLine("Waiting for client...");

            try
            {
                listener.Bind(localEP);
                listener.Listen(10);

                handler = listener.Accept();
                tw.WriteLine("Client Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Failed!   " + e);
                return false;
            }

            return true;
        }

        public static void sendMessage(String content) {
            try
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(content);
                int bytesSent = handler.Send(msg);
                if (content != "")
                {
                    if (content != "")
                    {
                        tw.WriteLine("Sending Message Content:\n" + content);
                    }
                }
      
            }
            catch (Exception e) {
                Console.WriteLine("Menssage Failed!");  
            }
        }

        public static void end() {
            try
            {
                tw.WriteLine("Terminating Server...");
                tw.Close();
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch { 
            }
        }

    }
}
