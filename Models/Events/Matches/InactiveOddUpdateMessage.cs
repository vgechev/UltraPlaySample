using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Matches
{
	public record InactiveOddUpdateMessage(InactiveOddUpdateDTO MatchUpdate)
	{
		public static implicit operator EventMessage<InactiveOddUpdateMessage>(InactiveOddUpdateMessage model) => new("inactive-odd", model);
	}
}
