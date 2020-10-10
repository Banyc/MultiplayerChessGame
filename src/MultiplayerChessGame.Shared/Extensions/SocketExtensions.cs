using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
namespace MultiplayerChessGame.Shared.Extensions
{
    public static class SocketExtensions
    {
        public static void StartListen(this Socket socket, string ipAddress, int port)
        {
            IPAddress ipAddr = IPAddress.Parse(ipAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket listener = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Using Bind() method we associate a 
                // network address to the Server Socket 
                // All client that will connect to this  
                // Server Socket must know this network 
                // Address 
                listener.Bind(localEndPoint);

                // Using Listen() method we create  
                // the Client list that will want 
                // to connect to Server 
                listener.Listen(10);
            }
            catch (Exception)
            {
            }
        }
    }
}
