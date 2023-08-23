using UltraPlaySample.Models.DTOs.Responses;

namespace UltraPlaySample.Repositories.Interfaces
{
	public interface IMainRepository
	{
		Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatchesByHoursRange(int hoursRange);
		Task<GetMatchResponseModel> GetMatchResponseModelById(int id);
	}
}