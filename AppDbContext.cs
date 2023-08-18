using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Enums;

namespace UltraPlaySample
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Sport> Sports { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Match> Matches { get; set; }
		public DbSet<Bet> Bets { get; set; }
		public DbSet<Odd> Odds { get; set; }
		public DbSet<MatchType> MatchTypes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//modelBuilder.Entity<MatchType>()
			//	.Property(x => x.Id)
			//	.HasConversion(
			//		value => (short)value,
			//		value => (MatchTypesEnum)value
			//	);

			modelBuilder.Entity<Odd>()
				.Property(x => x.SpecialBetValue)
				.HasConversion(
					value => value == 0 ? null : value,
					value => value == null ? 0 : value
				);
		}
	}

	public class MatchType
	{
		[Key]
		public MatchTypesEnum Id { get; set; }
		public string Name { get; set; }
	}

	public class Odd
	{
		[Key]
		public int Id { get; set; }

		[NotNull]
		public string Name { get; set; }

		[NotNull, Precision(18, 6)]
		public decimal Value { get; set; }

		[Precision(18, 6)]
		public decimal? SpecialBetValue { get; set; }

		[NotNull]
		public int BetId { get; set; }

		[ForeignKey(nameof(BetId))]
		public virtual Bet Bet { get; set; }
	}

	public class Bet
	{
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

	public class Match
	{
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

	public class Event
	{
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

	public class Sport
	{
		[Key]
		public int Id { get; set; }

		[NotNull]
		public string Name { get; set; }

		public virtual ICollection<Event> Events { get; set; }
	}
}