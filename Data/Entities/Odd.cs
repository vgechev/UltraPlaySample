using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace UltraPlaySample.Data.Entities
{
    public class Odd
    {
        public Odd() { }
        public Odd(int id, string name, decimal value, decimal? specialBetValue, int betId)
        {
            Id = id;
            Name = name;
            Value = value;
            SpecialBetValue = specialBetValue;
            IsActive = true;
            BetId = betId;
        }

        [Key]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull, Precision(18, 6)]
        public decimal Value { get; set; }

        [Precision(18, 6)]
        public decimal? SpecialBetValue { get; set; }

		[NotNull]
		public bool IsActive { get; set; }

		[NotNull]
        public int BetId { get; set; }

        [ForeignKey(nameof(BetId))]
        public virtual Bet Bet { get; set; }
    }
}