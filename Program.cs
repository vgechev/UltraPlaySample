using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Data;
using UltraPlaySample.Repositories.Implementations;
using UltraPlaySample.Repositories.Interfaces;
using UltraPlaySample.Services.Implementations;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
			builder.Services
				.AddControllers().Services
				.AddEndpointsApiExplorer()
				.AddSwaggerGen()
				.AddHttpClient()
				.AddDbContext<AppDbContext>(o =>
				{
					o.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
					//o.UseLoggerFactory(LoggerFactory.Create(b => b.AddFilter(level => level != LogLevel.Information)));
				})
				.AddScoped<IXmlDataService, XmlDataService>()
				.AddScoped<IMatchesService, MatchesService>()
				.AddScoped<IMainRepository, MainRepository>()
				.AddHostedService<TimedHostedService>();

			WebApplication app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
				app.UseSwagger().UseSwaggerUI();

			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}