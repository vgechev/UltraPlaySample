using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Models.DTOs.Events
{
	public record InactiveMatchUpdateDTO(int Id, string Name, MatchTypesEnum MatchType);
}
