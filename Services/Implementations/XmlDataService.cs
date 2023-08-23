using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Common.Enums;
using UltraPlaySample.Data;
using UltraPlaySample.Data.Entities;
using UltraPlaySample.Models.DTOs.XML;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services.Implementations
{
	public class XmlDataService : IXmlDataService
	{
		private readonly AppDbContext _dbContext;
		private readonly ILogger<XmlDataService> _logger;
		private readonly IHttpClientFactory _httpClientFactory;

		private Sport[] _dbSports;
		private Event[] _dbEvents;
		private Match[] _dbMatches;
		private Bet[] _dbBets;
		private Odd[] _dbOdds;

		public XmlDataService(AppDbContext dbContext, ILogger<XmlDataService> logger, IHttpClientFactory httpClientFactory)
		{
			_dbContext = dbContext;
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<XmlSports> FetchXmlData()
		{
			using HttpClient httpClient = _httpClientFactory.CreateClient();
			const string uri = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";
			HttpResponseMessage response = await httpClient.GetAsync(uri);

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogError("Failed to pull eSports data - bad response, reason: {reason}", await response.Content.ReadAsStringAsync());
				return null;
			}

			using Stream xmlStream = await response.Content.ReadAsStreamAsync();
			using StreamReader reader = new(xmlStream, Encoding.UTF8, true);
			XmlSerializer xmlSerializer = new(typeof(XmlSports), new XmlRootAttribute("XmlSports"));
			return (XmlSports)xmlSerializer.Deserialize(reader);
		}

		private async Task ProcessSport(XmlSport xmlSport)
		{
			Sport sport = _dbSports.FirstOrDefault(s => s.Id == xmlSport.Id);

			if (sport is null)
			{
				sport = new(xmlSport.Id, xmlSport.Name, await ProcessEvents(xmlSport.Events, xmlSport.Id));
				_dbContext.Sports.Add(sport);
				return;
			}

			sport.Id = xmlSport.Id;
			sport.Name = xmlSport.Name;
			sport.Events = await ProcessEvents(xmlSport.Events, xmlSport.Id);
			_dbContext.Sports.Update(sport);
		}

		private async Task<List<Event>> ProcessEvents(IEnumerable<XmlEvent> xmlEvents, int sportId)
		{
			List<Event> addedEvents = new();

			foreach (XmlEvent xmlEvent in xmlEvents)
			{
				Event @event = _dbEvents.FirstOrDefault(e => e.Id == xmlEvent.Id);

				if (@event is null)
				{
					@event = new(xmlEvent.Id, xmlEvent.Name, xmlEvent.IsLive, xmlEvent.CategoryID, sportId, await ProcessMatches(xmlEvent.Matches, xmlEvent.Id));
					_dbContext.Events.Add(@event);
					addedEvents.Add(@event);
					continue;
				}

				@event.Id = xmlEvent.Id;
				@event.Name = xmlEvent.Name;
				@event.IsLive = xmlEvent.IsLive;
				@event.CategoryId = xmlEvent.CategoryID;
				@event.SportId = sportId;
				@event.Matches = await ProcessMatches(xmlEvent.Matches, xmlEvent.Id);
				addedEvents.Add(@event);
			}

			return addedEvents;
		}

		private async Task<List<Match>> ProcessMatches(IEnumerable<XmlMatch> xmlMatches, int eventId)
		{
			List<Match> addedMatches = new();

			foreach (XmlMatch xmlMatch in xmlMatches)
			{
				if (xmlMatch.MatchType == MatchTypesEnum.OutRight)
					continue;

				Match match = _dbMatches.FirstOrDefault(m => m.Id == xmlMatch.Id);

				if (match is null)
				{
					match = new(xmlMatch.Id, xmlMatch.Name, xmlMatch.StartDate, xmlMatch.MatchType, eventId, await ProcessBets(xmlMatch.Bets, xmlMatch.Id));
					_dbContext.Matches.Add(match);
					addedMatches.Add(match);
					continue;
				}

				match.Id = xmlMatch.Id;
				match.Name = xmlMatch.Name;
				match.EventId = eventId;
				match.Bets = await ProcessBets(xmlMatch.Bets, xmlMatch.Id);
				addedMatches.Add(match);
			}

			return addedMatches;
		}

		private async Task<List<Bet>> ProcessBets(ICollection<XmlBet> xmlBets, int matchId)
		{
			List<Bet> addedBets = new();

			foreach (XmlBet xmlBet in xmlBets)
			{
				Bet bet = _dbBets.FirstOrDefault(m => m.Id == xmlBet.Id);

				if (bet is null)
				{
					bet = new(xmlBet.Id, xmlBet.Name, xmlBet.IsLive, matchId, await ProcessOdds(xmlBet.Odds, xmlBet.Id));
					_dbContext.Bets.Add(bet);
					addedBets.Add(bet);
					continue;
				}

				bet.Id = xmlBet.Id;
				bet.Name = xmlBet.Name;
				bet.IsLive = xmlBet.IsLive;
				bet.MatchId = matchId;
				bet.Odds = await ProcessOdds(xmlBet.Odds, xmlBet.Id);
				addedBets.Add(bet);
			}

			return addedBets;
		}

		private async Task<List<Odd>> ProcessOdds(IEnumerable<XmlOdd> xmlOdds, int betId)
		{
			List<Odd> processedOdds = new();

			foreach (XmlOdd xmlOdd in xmlOdds)
			{
				Odd odd = _dbOdds.FirstOrDefault(m => m.Id == xmlOdd.Id);

				if (odd is null)
				{
					odd = new(xmlOdd.Id, xmlOdd.Name, xmlOdd.Value, xmlOdd.SpecialBetValue, betId);
					_dbContext.Odds.Add(odd);
					processedOdds.Add(odd);
					continue;
				}

				odd.Id = xmlOdd.Id;
				odd.Name = xmlOdd.Name;
				odd.Value = xmlOdd.Value;
				odd.SpecialBetValue = xmlOdd.SpecialBetValue;
				odd.BetId = betId;
				processedOdds.Add(odd);
			}

			return processedOdds;
		}

		public async Task ProcessDataAndSaveToDatabase(XmlSport[] xmlSports)
		{
			// TODO: Don't store the following tables in memory
			_dbSports = await _dbContext.Sports.ToArrayAsync();
			_dbEvents = await _dbContext.Events.ToArrayAsync();
			_dbMatches = await _dbContext.Matches.ToArrayAsync();
			_dbBets = await _dbContext.Bets.ToArrayAsync();
			_dbOdds = await _dbContext.Odds.ToArrayAsync();

			try
			{
				foreach (XmlSport xmlSport in xmlSports)
					await ProcessSport(xmlSport);

				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to save xml data in database with exception: {msg}", ex.Message);
			}
		}
	}
}