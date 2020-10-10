using System;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace MultiplayerChessGame.Server.Services
{
    public class GameServer : TcpServer
    {
        private readonly ChessGameManagerService _gameManager;
        public GameServer(IPAddress address, int port, ChessGameManagerService gameManager) : base(address, port)
        {
            _gameManager = gameManager;
        }

        protected override TcpSession CreateSession() { return new GameSession(this, _gameManager); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }
}
