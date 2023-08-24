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
				return null; // TODO: Return an error code (msg) and handle this case appropriately.

			switch (match.MatchType)
			{
				case MatchTypesEnum.PreMatch:
					await SetBetsForPreMatchMatch(matchId, match);
					break;

				case MatchTypesEnum.Live:
					await SetBetsForLiveMatch(matchId, match);
					break;

				case MatchTypesEnum.OutRight:
					throw new DataException("There shouldn't be any OutRight matches");

				default:
					throw new ArgumentException("Match has unknown match type");
			}

			return match;
		}

		private async Task SetBetsForLiveMatch(int matchId, GetMatchResponseModel match)
		{
			if (match.StartDate >= DateTime.UtcNow) // TODO: figure out when is a bet no longer considered 'active' for Live matches
				match.ActiveBets = await GetBetsWithOddsForMatch(matchId);
			else
				match.InactiveBets = await GetBetsWithOddsForMatch(matchId);
		}

		private async Task SetBetsForPreMatchMatch(int matchId, GetMatchResponseModel match)
		{
			if (match.StartDate <= DateTime.UtcNow)
				match.ActiveBets = await GetBetsWithOddsForMatch(matchId);
			else
				match.InactiveBets = await GetBetsWithOddsForMatch(matchId);
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
					.Where(b => _previewBetNames.Contains(b.Name))
					.Select(b => new BetDto(b.Name, b.IsLive, GetOddsDtosFromEntities(b.Odds)))
					.ToArray()))
				.ToArrayAsync();
		}

		private static OddDto[] GetOddsDtosFromEntities(ICollection<Odd> odds) =>
			odds.Select(o => new OddDto(o.Name, o.SpecialBetValue, o.Value)).ToArray();
	}
}