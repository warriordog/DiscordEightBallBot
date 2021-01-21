using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordEightBallBot
{
    public class EightBallBotService : IHostedService
    {
        private readonly EightBallBotMain _eightBallBotMain;

        public EightBallBotService(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _eightBallBotMain = scope.ServiceProvider.GetRequiredService<EightBallBotMain>();
        }

        public Task StartAsync(CancellationToken _) => _eightBallBotMain.StartAsync();
        public Task StopAsync(CancellationToken _) => _eightBallBotMain.StopAsync();
    }
}