using System.Xml;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace MultiplayerChessGame.Server.Services
{
    public class GameServerService : IHostedService
    {
        private readonly GameServer _gameServer;
        private readonly ILogger<GameServerService> _logger;
        public GameServerService(ChessGameManagerService gameManager, ILogger<GameServerService> logger)
        {
            _logger = logger;
            int port = 18652;
            _gameServer = new GameServer(IPAddress.Any, port, gameManager);
            _logger.LogInformation($"Started on port {port}");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _gameServer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
