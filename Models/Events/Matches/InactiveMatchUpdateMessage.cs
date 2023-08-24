using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Matches
{
	public record InactiveMatchUpdateMessage(InactiveMatchUpdateDTO MatchUpdate)
	{
		public static implicit operator EventMessage<InactiveMatchUpdateMessage>(InactiveMatchUpdateMessage model) => new("inactive-match", model);
	}
}
