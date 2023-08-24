using UltraPlaySample.Models.DTOs.Events;

namespace UltraPlaySample.Models.Events.Bets
{
    public record BetUpdateMessage(BetUpdateDTO BetUpdate)
    {
        public static implicit operator EventMessage<BetUpdateMessage>(BetUpdateMessage model) => new("bet-update", model);
    }
}
