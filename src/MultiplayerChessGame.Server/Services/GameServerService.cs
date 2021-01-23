using System.Xml;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using MultiplayerChessGame.Server.Models;
using Microsoft.Extensions.Options;

namespace MultiplayerChessGame.Server.Services
{
    public class GameServerService : IHostedService
    {
        private readonly GameServer _gameServer;
        private readonly ILogger<GameServerService> _logger;
        public GameServerService(ChessGameManagerService gameManager, IOptions<GameServerServiceOptions> options, ILogger<GameServerService> logger)
        {
            _logger = logger;
            _gameServer = new GameServer(IPAddress.Any, options.Value.Port, gameManager);
            _logger.LogInformation($"Started on port {options.Value.Port}");
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
