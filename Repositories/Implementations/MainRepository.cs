using System.Data;
using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Common.Enums;
using UltraPlaySample.Data;
using UltraPlaySample.Data.Entities;
using UltraPlaySample.Models.DTOs;
using UltraPlaySample.Models.DTOs.Responses;
using UltraPlaySample.Repositories.Interfaces;

namespace UltraPlaySample.Repositories.Implementations
{
	public class MainRepository : IMainRepository
	{
		private readonly AppDbContext _dbContext;
		private readonly string[] _previewBetNames = new string[] { "Match Winner", "Map Advantage", "Total Maps Played" };

		public MainRepository(AppDbContext appDbContext) => _dbContext = appDbContext;

		public async Task<GetMatchResponseModel> GetMatchResponseModelById(int matchId)
		{
			GetMatchResponseModel match = await _dbContext.Matches
				.Where(m => m.Id == matchId)
				.Select(m => new GetMatchResponseModel(m.Name, m.MatchType, m.StartDate))
				.FirstOrDefaultAsync();

			if (match is null)
				return null; // TODO: Return an error code (msg) and handle appropriately.

			DateTime now = DateTime.UtcNow;

			switch (match.MatchType)
			{
				case MatchTypesEnum.PreMatch:
					if (match.StartDate <= now)
						match.ActiveBets = await GetBetsWithOddsForMatch(matchId);
					else
						match.InactiveBets = await GetBetsWithOddsForMatch(matchId);
					break;

				case MatchTypesEnum.Live:
					if (match.StartDate.Date == now.Date && match.StartDate >= now) // TODO: figure out the "EndDate" condition
						match.ActiveBets = await GetBetsWithOddsForMatch(matchId);
					else
						match.InactiveBets = await GetBetsWithOddsForMatch(matchId);

					break;

				case MatchTypesEnum.OutRight:
					throw new DataException("There shouldn't be any OutRight matches");

				default:
					break;
			}

			return match;
		}

		private async Task<BetDto[]> GetBetsWithOddsForMatch(int matchId) => await _dbContext.Bets
									.Where(b => b.MatchId == matchId)
									.Select(b => new BetDto(b.Name, b.IsLive, GetOddsDtosFromEntities(b.Odds)))
									.ToArrayAsync();

		public async Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatchesByHoursRange(int hoursRange)
		{
			DateTime now = DateTime.UtcNow;

			return await _dbContext.Matches
				.Where(m => m.StartDate >= now && m.StartDate <= now.AddHours(hoursRange))
				.OrderByDescending(m => m.StartDate)
				.Select(m => new GetUpcomingMatchResponseModel(m.Name, m.StartDate, m.Bets
					.Where(b => _previewBetNames.Contains(b.Name, StringComparer.OrdinalIgnoreCase))
					.Select(b => new BetDto(b.Name, b.IsLive, GetOddsDtosFromEntities(b.Odds)))
					.ToArray()))
				.ToArrayAsync();
		}

		private static OddDto[] GetOddsDtosFromEntities(ICollection<Odd> odds) =>
			odds.Select(o => new OddDto(o.Name, o.SpecialBetValue, o.Value)).ToArray();
	}
}

/*
## Active Bets
Active bets refer to betting opportunities that are currently available for customers to place bets on:

- For "Prematch" matches (matches that are open for betting before their start date), active bets are those that are currently open for 
betting before the match's start date.

- For "Live" matches (matches that have started), active bets are those that are currently open for betting after the match's start date.

- The term "active" in this context doesn't necessarily mean that a specific bet has ongoing activity or bets placed on it; 
it simply indicates that the bet is currently available for customers to place bets on.

## Inactive Bets
Inactive bets, on the other hand, refer to betting opportunities that are no longer available for customers to place bets on. 
They might have become inactive due to various reasons, such as the match ending, the betting window closing, or the outcome of the bet being resolved.
*/