using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Matches
{
	public record MatchUpdateMessage(MatchUpdateDTO MatchUpdate)
	{
		public static implicit operator EventMessage<MatchUpdateMessage>(MatchUpdateMessage model) => new("match-update", model);
	}
}