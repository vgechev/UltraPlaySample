namespace UltraPlaySample.Services.Interfaces
{
    public interface IMatchesService
    {
		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches();
	}
}