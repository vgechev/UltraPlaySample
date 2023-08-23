using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UltraPlaySample.Data.Entities
{
    public class Sport
    {
        [Key]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public Sport() { }
        public Sport(int id, string name, ICollection<Event> events)
        {
            Id = id;
            Name = name;
            Events = events;
        }
    }
}