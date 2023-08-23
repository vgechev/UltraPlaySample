using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Data.Entities
{
    public class Match
    {
        public Match() { }
        public Match(int id, string name, DateTime startDate, MatchTypesEnum matchType, int eventId, ICollection<Bet> bets)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            MatchType = matchType;
            EventId = eventId;
            Bets = bets;
        }

        [Key]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public DateTime StartDate { get; set; }

        [NotNull]
        public MatchTypesEnum MatchType { get; set; }

        [NotNull]
        public int EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }
    }
}