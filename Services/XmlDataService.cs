using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Models;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services
{
	public class XmlDataService : IXmlDataService
	{
		private readonly AppDbContext dbContext;
		private readonly ILogger<XmlDataService> logger;
		private readonly IHttpClientFactory httpClientFactory;
		private Sport[] dbSports;
		private Event[] dbEvents;
		private Match[] dbMatches;
		private Bet[] dbBets;
		private Odd[] dbOdds;

		public XmlDataService(AppDbContext dbContext, ILogger<XmlDataService> logger, IHttpClientFactory httpClientFactory)
		{
			this.dbContext = dbContext;
			this.logger = logger;
			this.httpClientFactory = httpClientFactory;
		}

		public async Task<XmlSports> FetchXmlData()
		{
			using HttpClient httpClient = httpClientFactory.CreateClient();
			const string uri = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";
			HttpResponseMessage response = await httpClient.GetAsync(uri);

			if (!response.IsSuccessStatusCode)
			{
				logger.LogError("Failed to pull eSports data - bad response, reason: {reason}", await response.Content.ReadAsStringAsync());
				return null;
			}

			using Stream xmlStream = await response.Content.ReadAsStreamAsync();
			using StreamReader reader = new(xmlStream, Encoding.UTF8, true);
			XmlSerializer xmlSerializer = new(typeof(XmlSports), new XmlRootAttribute("XmlSports"));
			return (XmlSports)xmlSerializer.Deserialize(reader);
		}

		private Sport CreateSport(XmlSport xmlSport)
		{
			Sport sport = dbSports.FirstOrDefault(s => s.Id == xmlSport.Id) ?? new();

			sport.Id = xmlSport.Id;
			sport.Name = xmlSport.Name;
			sport.Events = CreateEvents(xmlSport.Events);

			return sport;
		}

		private List<Event> CreateEvents(IEnumerable<XmlEvent> xmlEvents)
		{
			List<Event> events = new();

			foreach (XmlEvent xmlEvent in xmlEvents)
			{
				Event @event = dbEvents.FirstOrDefault(e => e.Id == xmlEvent.Id) ?? new();

				@event.Id = xmlEvent.Id;
				@event.Name = xmlEvent.Name;
				@event.IsLive = xmlEvent.IsLive;
				@event.CategoryId = xmlEvent.CategoryID;
				@event.Matches = CreateMatches(xmlEvent.Matches);

				events.Add(@event);
			}

			return events;
		}

		private List<Match> CreateMatches(IEnumerable<XmlMatch> xmlMatches)
		{
			List<Match> matches = new();

			foreach (XmlMatch xmlMatch in xmlMatches)
			{
				Match match = dbMatches.FirstOrDefault(m => m.Id == xmlMatch.Id) ?? new();

				match.Id = xmlMatch.Id;
				match.Name = xmlMatch.Name;
				match.StartDate = xmlMatch.StartDate;
				match.MatchType = xmlMatch.MatchType;
				match.Bets = CreateBets(xmlMatch.Bets);

				matches.Add(match);
			}

			return matches;
		}

		private List<Bet> CreateBets(IEnumerable<XmlBet> xmlBets)
		{
			List<Bet> bets = new();

			foreach (XmlBet xmlBet in xmlBets)
			{
				Bet bet = dbBets.FirstOrDefault(b => b.Id == xmlBet.Id) ?? new();

				bet.Id = xmlBet.Id;
				bet.Name = xmlBet.Name;
				bet.IsLive = xmlBet.IsLive;
				bet.Odds = CreateOdds(xmlBet.Odds);

				bets.Add(bet);
			}

			return bets;
		}

		private List<Odd> CreateOdds(IEnumerable<XmlOdd> xmlOdds)
		{
			List<Odd> odds = new();

			foreach (XmlOdd xmlOdd in xmlOdds)
			{
				Odd odd = dbOdds.FirstOrDefault(o => o.Id == xmlOdd.Id) ?? new();

				odd.Id = xmlOdd.Id;
				odd.Name = xmlOdd.Name;
				odd.Value = xmlOdd.Value;
				odd.SpecialBetValue = xmlOdd.SpecialBetValue;

				odds.Add(odd);
			}

			return odds;
		}

		public async Task ProcessDataAndSaveToDatabase(XmlSport[] sports)
		{
			dbSports = await dbContext.Sports.ToArrayAsync();
			dbEvents = await dbContext.Events.ToArrayAsync();
			dbMatches = await dbContext.Matches.Where(m => m.StartDate >= DateTime.UtcNow).ToArrayAsync();
			dbBets = await dbContext.Bets.ToArrayAsync();
			dbOdds = await dbContext.Odds.ToArrayAsync();

			List<Sport> sportEntities = new();

			try
			{
				foreach (XmlSport xmlSport in sports)
				{
					Sport sport = CreateSport(xmlSport);
					sportEntities.Add(sport);
				}

				dbContext.Sports.AddRange(sportEntities);
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				logger.LogError("Failed to save xml data in database with message: ", ex.Message);
			}
		}
	}
}