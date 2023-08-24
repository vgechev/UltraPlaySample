using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Odds
{
	public record OddUpdateMessage(OddUpdateDTO OddUpdate)
	{
		public static implicit operator EventMessage<OddUpdateMessage>(OddUpdateMessage model) => new("odd-update", model);
	}
}
