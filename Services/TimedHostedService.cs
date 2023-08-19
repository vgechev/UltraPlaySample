using UltraPlaySample.Models;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services
{
    public class TimedHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<TimedHostedService> logger;
		private readonly IServiceScopeFactory factory;
		private Timer pullingTimer;

		public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory factory)
		{
			this.logger = logger;
			this.factory = factory;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			pullingTimer = new(async (_) => await GetESportsData(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
			return Task.CompletedTask;
		}

		private async Task GetESportsData()
		{
			try
			{
				using IServiceScope scope = factory.CreateScope();
				IXmlDataService xmlDataService = scope.ServiceProvider.GetRequiredService<IXmlDataService>();
				XmlSports xml = await xmlDataService.FetchXmlData();
				await xmlDataService.ProcessDataAndSaveToDatabase(xml.Sports);
			}
			catch (Exception ex)
			{
				logger.LogError("Failed to pull eSports data - {exception}", ex);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			pullingTimer?.Dispose();
			return Task.CompletedTask;
		}

		public void Dispose() => pullingTimer?.Dispose();
	}
}