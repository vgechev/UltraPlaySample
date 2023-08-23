using UltraPlaySample.Models.DTOs;
using UltraPlaySample.Models.DTOs.Responses;
using UltraPlaySample.Repositories.Interfaces;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services.Implementations
{
	public class MatchesService : IMatchesService
	{
		private readonly IMainRepository _mainRepository;
		public MatchesService(IMainRepository mainRepository) => _mainRepository = mainRepository;

		public async Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches()
		{
			GetUpcomingMatchResponseModel[] matches = await _mainRepository.GetUpcomingMatchesByHoursRange(24);

			foreach (GetUpcomingMatchResponseModel match in matches)
				foreach (BetDto bet in match.Bets.Where(x => x.Odds.All(o => o.SpecialBetValue.HasValue)).ToArray())
					bet.Odds = bet.Odds
						.GroupBy(o => o.SpecialBetValue)
						.Select(g => g.First())
						.ToArray();

			return matches;
		}

		public Task<GetMatchResponseModel> GetMatchById(int id) =>
			_mainRepository.GetMatchResponseModelById(id);
	}
}