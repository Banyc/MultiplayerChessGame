using System.Net;
using System.Net.Sockets;

namespace MultiplayerChessGame.Shared.Helpers
{
    public static class SocketHelpers
    {
        public static Socket GetListener(string ipAddress, int port)
        {
            IPAddress ipAddr = IPAddress.Parse(ipAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket listener = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);

            return listener;
        }
    }
}
