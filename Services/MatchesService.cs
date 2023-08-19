using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services
{
	public class MatchesService : IMatchesService
	{
		private readonly AppDbContext dbContext;
		private readonly string[] previewBetNames = new string[] { "Match Winner", "Map Advantage", "Total Maps Played" };

		public MatchesService(AppDbContext dbContext) => this.dbContext = dbContext;

		public async Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches()
		{
			DateTime now = DateTime.UtcNow;

			GetUpcomingMatchResponseModel[] result = await dbContext.Matches
				.Where(m => m.StartDate > now && m.StartDate <= now.AddHours(24))
				.OrderByDescending(m => m.StartDate)
				.Select(m => new GetUpcomingMatchResponseModel(m.Name, m.StartDate, m.Bets
					.Where(b => previewBetNames.Contains(b.Name))
					.Select(b => new BetDto(b.Name, b.IsLive, b.Odds
						.Select(o => new OddDto(o.Name, o.SpecialBetValue, o.Value)).ToArray()))
					.ToArray()))
				.ToArrayAsync();

			foreach (GetUpcomingMatchResponseModel r in result)
				foreach (BetDto bet in r.Bets.Where(x => x.Odds.All(o => o.SpecialBetValue.HasValue)).ToArray())
					bet.Odds = bet.Odds
						.GroupBy(o => o.SpecialBetValue)
						.Select(g => g.First())
						.ToArray();

			return result;
		}

		public Task<GetMatchResponseModel> GetMatchById(int id) =>
			dbContext.Matches.Where(m => m.Id == id)
				.Select(m => new GetMatchResponseModel(m.Name, m.StartDate, m.Bets
					.Select(b => new BetDto(b.Name, b.IsLive, b.Odds
						.Select(o => new OddDto(o.Name, o.SpecialBetValue, o.Value)).ToArray()))
					.ToArray()))
				.FirstOrDefaultAsync();
	}

	public record GetUpcomingMatchResponseModel(string Name, DateTime StartDate, BetDto[] Bets);
	public record GetMatchResponseModel(string Name, DateTime StartDate, BetDto[] Bets);


	public record BetDto(string Name, bool IsLive)
	{
		public BetDto(string Name, bool IsLive, OddDto[] Odds)
			: this(Name, IsLive) => this.Odds = Odds;

		public OddDto[] Odds { get; set; }
	};

	public record OddDto(string Name, decimal? SpecialBetValue, decimal Value)
	{
		public static OddDto GetDtoFromEntity(Odd entity) => new(entity.Name, entity.SpecialBetValue, entity.Value);
	}
}