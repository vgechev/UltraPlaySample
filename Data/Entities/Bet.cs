using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UltraPlaySample.Data.Entities
{
    public class Bet
    {
        public Bet() { }
        public Bet(int id, string name, bool isLive, int matchId, ICollection<Odd> odds)
        {
            Id = id;
            Name = name;
            IsLive = isLive;
            MatchId = matchId;
            Odds = odds;
        }

        [Key]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public bool IsLive { get; set; }

        [NotNull]
        public int MatchId { get; set; }

        [ForeignKey(nameof(MatchId))]
        public virtual Match Match { get; set; }

        public virtual ICollection<Odd> Odds { get; set; }
    }
}