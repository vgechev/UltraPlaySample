using Microsoft.AspNetCore.Mvc;
using UltraPlaySample.Services;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Controllers
{
	[ApiController, Route("[controller]")]
	public class MatchesController : ControllerBase
	{
		private readonly ILogger<MatchesController> logger;
		private readonly IMatchesService matchesService;

		public MatchesController(ILogger<MatchesController> logger, IMatchesService matchesService)
		{
			this.logger = logger;
			this.matchesService = matchesService;
		}

		[HttpGet("upcoming")]
		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches() =>
			matchesService.GetUpcomingMatches();
	}
}