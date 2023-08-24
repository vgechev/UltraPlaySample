using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Common.Enums;
using UltraPlaySample.Data.Entities;
using MatchType = UltraPlaySample.Data.Entities.MatchType;

namespace UltraPlaySample.Data
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

			modelBuilder.Entity<MatchType>().HasData(new MatchType[]
			{
				new(MatchTypesEnum.PreMatch, MatchTypesEnum.PreMatch.ToString()),
				new(MatchTypesEnum.Live, MatchTypesEnum.Live.ToString()),
				new(MatchTypesEnum.OutRight, MatchTypesEnum.OutRight.ToString())
			});

			modelBuilder.Entity<Odd>()
				.Property(x => x.SpecialBetValue)
				.HasConversion(
					value => value == 0 ? null : value,
					value => value
				);
		}
	}
}