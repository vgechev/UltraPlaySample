using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UltraPlaySample.Data.Entities
{
    public class Event
    {
        public Event() { }
        public Event(int id, string name, bool isLive, int categoryId, int sportId, ICollection<Match> matches)
        {
            Id = id;
            Name = name;
            IsLive = isLive;
            CategoryId = categoryId;
            SportId = sportId;
            Matches = matches;
        }

        [Key]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public bool IsLive { get; set; }

        [NotNull]
        public int CategoryId { get; set; }

        [NotNull]
        public int SportId { get; set; }

        [ForeignKey(nameof(SportId))]
        public virtual Sport Sport { get; set; }

        public virtual ICollection<Match> Matches { get; set; }
    }
}