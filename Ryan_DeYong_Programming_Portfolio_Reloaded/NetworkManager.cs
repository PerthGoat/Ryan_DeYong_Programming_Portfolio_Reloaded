using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ryan_DeYong_Programming_Portfolio_Reloaded
{
    class NetworkManager
    {
        /*
         * Creates a new NetworkManager with the port specified
         * */
        public NetworkManager(int port) {
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Loopback;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100); // 100 max connections

                while (true)
                {
                    allDone.Reset();

                    //Console.WriteLine("Waiting for a connection..");
                    listener.BeginAccept(new AsyncCallback(AsyncCallbackFunc), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue..");
            Console.Read();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                LogMessage(handler, "Sent " + bytesSent + " bytes to client.");

                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket handler, byte[] data)
        {
            handler.BeginSend(data, 0, data.Length, 0,
            new AsyncCallback(SendCallback), handler);
        }

        private static void LogMessage(Socket handler, string error) {
            Console.WriteLine(handler.RemoteEndPoint + " :: " + error);
        }

        private static byte[] BuildHTMLResponse(string filename) {
            string response = "HTTP/1.1 200 OK\nContent-Type: text/html; charset=ascii\nConnection: Closed\n";
            byte[] file_text = File.ReadAllBytes(filename);

            string file_type = filename.Split('.').Last().ToUpper();

            if (file_type == "HTML")
            {
                TemplateEngine te = new TemplateEngine();
                file_text = Encoding.ASCII.GetBytes(te.ReplaceTemplatesInString(Encoding.ASCII.GetString(file_text)));
            }

            response += "Content-Length: " + file_text.Length + "\n\n";

            return Encoding.ASCII.GetBytes(response).Concat(file_text).ToArray();
        }

        private static void ServeFile(Socket handler, string filename) {
            if (filename == "/") {
                filename = "/index.html";
            }

            filename = "master_directory" + filename;

            LogMessage(handler, "Preparing to serve file " + filename);

            // check for if someone tries to escape the directory

            DirectoryInfo server_dir = new DirectoryInfo("master_directory");
            DirectoryInfo cur_dir = new DirectoryInfo(filename);

            if (!cur_dir.FullName.StartsWith(server_dir.FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                LogMessage(handler, "Requested file OUTSIDE of the handling directory! Sending a 404!");
                Send(handler, Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\nRefresh:0\nConnection: Closed\n"));
                return;
            }

            if (!File.Exists(filename))
            {
                LogMessage(handler, "Requested invalid file " + filename + ". Sending 404 now.");
                Send(handler, Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found\nRefresh:0\nConnection: Closed\n"));
                return;
            }
            else {
                Send(handler, BuildHTMLResponse(filename));
            }
        }

        private static void ReadCallback(IAsyncResult ar) {
            object[] passedValues = (object[])ar.AsyncState;
            Socket handler = (Socket)passedValues[1];
            int bytes_input = handler.EndReceive(ar);

            LogMessage(handler, "Received " + bytes_input + " byte header!");
            string in_req = Encoding.ASCII.GetString((byte[])passedValues[0]);
            string in_get = in_req.Split('\n')[0];

            if (!in_get.StartsWith("GET")) {
                LogMessage(handler, "Invalid formed request!");
                return;
            }

            string[] in_params = in_get.Split(' ');
            string filename = in_params[1];

            LogMessage(handler, "Asking for file " + filename);

            ServeFile(handler, filename);
        }

        private static void AsyncCallbackFunc(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            byte[] requestString = new byte[1024];

            // Create the state object.  
            handler.BeginReceive(requestString, 0, 1024, 0,
                new AsyncCallback(ReadCallback), new object[] { requestString, handler });

            //Send(handler, "HTTP/1.1 200 OK\nContent-Type: text/html; charset=ascii\nConnection: Closed\nContent-Length: 1\n\nH");

            LogMessage(handler, "New Connection");
        }


        private static ManualResetEvent allDone = new ManualResetEvent(false);
    }
}
