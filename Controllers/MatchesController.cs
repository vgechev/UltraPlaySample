using Microsoft.AspNetCore.Mvc;
using UltraPlaySample.Services;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Controllers
{
	[ApiController, Route("[controller]")]
	public class MatchesController : ControllerBase
	{
		private readonly IMatchesService matchesService;
		public MatchesController(IMatchesService matchesService) => this.matchesService = matchesService;

		[HttpGet("upcoming")]
		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches() =>
			matchesService.GetUpcomingMatches();

		[HttpGet("{id:int}")]
		public Task<GetMatchResponseModel> GetMatchById(int id) =>
			matchesService.GetMatchById(id);
	}
}