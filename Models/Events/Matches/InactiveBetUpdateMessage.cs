using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Matches
{
	public record InactiveBetUpdateMessage(InactiveBetUpdateDTO MatchUpdate)
	{
		public static implicit operator EventMessage<InactiveBetUpdateMessage>(InactiveBetUpdateMessage model) => new("inactive-bet", model);
	}
}
