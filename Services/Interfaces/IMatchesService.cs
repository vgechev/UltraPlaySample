namespace UltraPlaySample.Services.Interfaces
{
    public interface IMatchesService
    {
		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches();
		public Task<GetMatchResponseModel> GetMatchById(int id);
	}
}