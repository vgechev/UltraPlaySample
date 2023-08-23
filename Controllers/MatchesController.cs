using Microsoft.AspNetCore.Mvc;
using UltraPlaySample.Models.DTOs.Responses;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Controllers
{
	[ApiController, Route("[controller]")]
	public class MatchesController : ControllerBase
	{
		private readonly IMatchesService _matchesService;
		public MatchesController(IMatchesService matchesService) => _matchesService = matchesService;

		[HttpGet("upcoming")]
		public Task<GetUpcomingMatchResponseModel[]> GetUpcomingMatches() =>
			_matchesService.GetUpcomingMatches();

		[HttpGet("{id:int}")]
		public Task<GetMatchResponseModel> GetMatchById(int id) =>
			_matchesService.GetMatchById(id);
	}
}