using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultiplayerChessGame.Server.Services;

namespace MultiplayerChessGame.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices(ConfigureServices);

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<GameServerService>();
            services.AddSingleton<ChessGameManagerService>();
        }
    }
}
