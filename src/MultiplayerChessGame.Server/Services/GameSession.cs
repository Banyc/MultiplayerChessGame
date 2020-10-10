using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MultiplayerChessGame.Shared.Helpers;
using MultiplayerChessGame.Shared.Models;
using NetCoreServer;

namespace MultiplayerChessGame.Server.Services
{
    public class GameSession : TcpSession
    {
        private readonly ChessGameManagerService _gameManager;
        private readonly RemoteInstructionProtocol _protocol;

        public GameSession(TcpServer server, ChessGameManagerService gameManager) : base(server)
        {
            _gameManager = gameManager;
            _protocol = new RemoteInstructionProtocol(gameManager.SharedGameState);
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

            // // Send invite message
            // string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            // SendAsync(message);

            // push game state to the new comer
            PushSharedGameState();
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // update game state on server side
            _protocol.ToHigherLayer(buffer, offset, size, this.OtherInstruction);

            // Multicast message to all connected sessions
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }

        private void OtherInstruction(byte[] frame)
        {
            RemoteInstruction instruction = JsonSerializer.Deserialize<RemoteInstruction>(frame);
            switch (instruction.Type)
            {
                case RemoteInstructionType.PullSharedGameState:
                    PushSharedGameState();
                    break;
            }
        }

        private void PushSharedGameState()
        {
            PushSharedGameState pushInstruction = new PushSharedGameState()
            {
                SharedGameState = _gameManager.SharedGameState,
            };
            byte[] rawBytes = _protocol.ToLowerLayer(pushInstruction);
            this.Send(rawBytes);
        }
    }
}
