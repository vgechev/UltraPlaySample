using UltraPlaySample.Models.DTOs.Responses;

namespace UltraPlaySample.Services.Interfaces
{
	public interface IMatchesService
	{
		Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches();
		Task<GetMatchResponseModel> GetMatchById(int id);
	}
}