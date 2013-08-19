using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using WpfApplicationPropertyChanged;

namespace WPFTCPIPServer
{

    [ImplementPropertyChanged]
    public class ViewModel
    {
        public string Temperature { get; set; }
        public string ServerStatus { get; set; }

        public ViewModel()
        {
            ServerStatus = "Press Start server button to start the TCP/IP server";
        }

        void UpdateControlExecute()
        {
            Thread newThread = new Thread(new ThreadStart(StartServer));
            newThread.Start();
        }

        private void StartServer()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.2.101");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop. 
                while (true)
                {
                    ServerStatus = "Waiting for a connection... ";

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    ServerStatus = "Connected!";

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client. 
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Temperature = data;

                        // Process the data sent by the client.
                        //data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        ServerStatus = "Sent: " + data;
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        bool CanUpdateControlExecute()
        {
            return true;
        }

        public ICommand StartServerButton
        {
            get { return new RelayCommand(UpdateControlExecute, CanUpdateControlExecute); }
        }
    }
}
