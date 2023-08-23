using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Models.DTOs.Responses
{
	public class GetMatchResponseModel
	{
		public GetMatchResponseModel(string name, MatchTypesEnum matchType, DateTime startDate)
		{
			Name = name;
			MatchType = matchType;
			StartDate = startDate;
		}

		public string Name { get; set; }
		public MatchTypesEnum MatchType { get; set; }
		public DateTime StartDate { get; set; }
		public BetDto[] ActiveBets { get; set; } = Array.Empty<BetDto>();
		public BetDto[] InactiveBets { get; set; } = Array.Empty<BetDto>();
	}
}