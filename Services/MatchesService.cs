using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services
{
	public class MatchesService : IMatchesService
	{
		private readonly AppDbContext dbContext;

		public MatchesService(AppDbContext dbContext) => this.dbContext = dbContext;

		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches()
		{
			DateTime now = DateTime.UtcNow;

			return dbContext.Matches.Where(m => m.StartDate > now && m.StartDate <= now.AddHours(24))
				.Select(m => new GetUpcomingMatchResponseModel(m.Name, m.StartDate))
				.ToArrayAsync();
		}

		// all active preview bets with their active odds
	}

	public record GetUpcomingMatchResponseModel(string Name, DateTime StartDate);
}
