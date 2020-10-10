using System.Text.Json;
using MultiplayerChessGame.Shared.Models;
using MultiplayerChessGame.Shared.Protocol;

namespace MultiplayerChessGame.Client.Services
{
    public class GameClientService
    {
        public GameClient Client { get; }
        public GameClientService(SharedGameState gameState)
        {
            int port = 18652;
            string ipAddress = "127.0.0.1";
            this.Client = new GameClient(ipAddress, port, gameState);
            this.Client.ConnectAsync();
        }
    }
}
