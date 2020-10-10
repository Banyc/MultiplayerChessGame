using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MultiplayerChessGame.Server.Services
{
    public class GameConsoleService : IHostedService
    {
        private readonly ChessGameManagerService _gameManager;
        public GameConsoleService(ChessGameManagerService gameManager)
        {
            _gameManager = gameManager;
        }

        public void StartLoop()
        {
            while (true)
            {
                try
                {
                    string input;
                    input = Console.ReadLine();
                    string[] commands = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    switch (commands[0])
                    {
                        case "state":
                            string json = JsonSerializer.Serialize(_gameManager.SharedGameState);
                            Console.WriteLine(json);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartLoop();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
