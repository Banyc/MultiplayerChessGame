using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MultiplayerChessGame.Shared.Models;
using MultiplayerChessGame.Shared.Protocol;

namespace MultiplayerChessGame.Client.Services
{
    public class GameClientService
    {
        public GameClient Client { get; }
        public GameClientService(SharedGameState gameState, IConfiguration configuration)
        {
            int port = configuration.GetValue<int>("Port");
            string ipAddress = configuration.GetValue<string>("IpAddress");
            this.Client = new GameClient(ipAddress, port, gameState);
            this.Client.ConnectAsync();
        }
    }
}
