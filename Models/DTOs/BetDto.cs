namespace UltraPlaySample.Models.DTOs
{
    public record BetDto(string Name, bool IsLive)
    {
        public BetDto(string Name, bool IsLive, OddDto[] Odds)
            : this(Name, IsLive) => this.Odds = Odds;

        public OddDto[] Odds { get; set; }
    };
}