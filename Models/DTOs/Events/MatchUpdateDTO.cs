using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Models.DTOs.Events
{
	public record MatchUpdateDTO(int Id, string Name, DateTime StartDate, MatchTypesEnum MatchType);
}
