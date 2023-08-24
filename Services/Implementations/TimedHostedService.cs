using UltraPlaySample.Models.DTOs.XML;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services.Implementations
{
	public class TimedHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<TimedHostedService> logger;
		private readonly IServiceScopeFactory factory;
		private Timer xmlDataTimer;

		public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory factory)
		{
			this.logger = logger;
			this.factory = factory;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			xmlDataTimer = new(async (_) => await ProcessESportsData(), null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
			return Task.CompletedTask;
		}

		private async Task ProcessESportsData()
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
			xmlDataTimer?.Dispose();
			return Task.CompletedTask;
		}

		public void Dispose() => xmlDataTimer?.Dispose();
	}
}