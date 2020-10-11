using System.Runtime.Versioning;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using MultiplayerChessGame.Shared.Models;
using TcpClient = NetCoreServer.TcpClient;
using MultiplayerChessGame.Shared.Helpers;
using System.Collections.Generic;

namespace MultiplayerChessGame.Client.Services
{
    public class GameClient : TcpClient
    {
        private SharedGameState _gameState;
        private readonly RemoteInstructionProtocol _protocol;

        public GameClient(string address, int port, SharedGameState gameState) : base(address, port) {
            _gameState = gameState;
            _protocol = new RemoteInstructionProtocol(gameState);
        }

        public void Send(RemoteInstruction instruction)
        {
            byte[] rawBytes = _protocol.ToLowerLayer(instruction);
            this.Send(rawBytes);
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            _protocol.ToHigherLayer(buffer, offset, size, this.OtherInstruction);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;

        private void OtherInstruction(byte[] frame)
        {
            RemoteInstruction instruction = JsonSerializer.Deserialize<RemoteInstruction>(frame);
            switch (instruction.Type)
            {
                case RemoteInstructionType.PushSharedGameState:
                    PushSharedGameState pushInstruction = Newtonsoft.Json.JsonConvert.DeserializeObject<PushSharedGameState>(Encoding.UTF8.GetString(frame));
                    // Newtonsoft.Json does not deserialize properly, so to reverse the stack explicitly.
                    pushInstruction.SharedGameState.BoardHistory = new Stack<string>(pushInstruction.SharedGameState.BoardHistory);
                    pushInstruction.SharedGameState.BoardRedo = new Stack<string>(pushInstruction.SharedGameState.BoardRedo);
                    _gameState.OverwriteState(pushInstruction.SharedGameState);
                    break;
            }
        }
    }
}
