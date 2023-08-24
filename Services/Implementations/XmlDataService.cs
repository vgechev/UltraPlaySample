using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using UltraPlaySample.Common.Enums;
using UltraPlaySample.Data;
using UltraPlaySample.Data.Entities;
using UltraPlaySample.Models.DTOs.XML;
using UltraPlaySample.Models.Events.Bets;
using UltraPlaySample.Models.Events.Matches;
using UltraPlaySample.Models.Events.Odds;
using UltraPlaySample.Modules;
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

		private int[] _activeMatchIds;
		private int[] _activeBetIds;
		private int[] _activeOddIds;

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

		public async Task ProcessDataAndSaveToDatabase(XmlSport[] xmlSports)
		{
			await FetchCurrentEntitiesInMemory();
			FillActiveIds(xmlSports);

			List<Match> inactiveMatches = await _dbContext.Matches.Where(m => !_activeMatchIds.Contains(m.Id) && m.IsActive).ToListAsync();
			List<Bet> inactiveBets = await _dbContext.Bets.Where(b => !_activeBetIds.Contains(b.Id) && b.IsActive).ToListAsync();
			List<Odd> inactiveOdds = await _dbContext.Odds.Where(o => !_activeOddIds.Contains(o.Id) && o.IsActive).ToListAsync();

			try
			{
				foreach (XmlSport xmlSport in xmlSports)
					ProcessSport(xmlSport);

				await _dbContext.SaveChangesAsync();

				SetInactiveMatches(inactiveMatches);
				SetInactiveBets(inactiveBets);
				SetInactiveOdds(inactiveOdds);

				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to save xml data in database with exception: {msg}", ex.Message);
			}
		}

		private void FillActiveIds(XmlSport[] xmlSports)
		{
			_activeMatchIds = xmlSports.SelectMany(sport => sport.Events).SelectMany(evt => evt.Matches).Select(match => match.Id).ToArray();
			_activeBetIds = xmlSports.SelectMany(sport => sport.Events).SelectMany(evt => evt.Matches).SelectMany(match => match.Bets).Select(bet => bet.Id).ToArray();
			_activeOddIds = xmlSports.SelectMany(sport => sport.Events).SelectMany(evt => evt.Matches).SelectMany(match => match.Bets).SelectMany(bet => bet.Odds).Select(odd => odd.Id).ToArray();
		}

		private async Task FetchCurrentEntitiesInMemory()
		{
			_dbSports = await _dbContext.Sports.ToArrayAsync();
			_dbEvents = await _dbContext.Events.ToArrayAsync();
			_dbMatches = await _dbContext.Matches.ToArrayAsync();
			_dbBets = await _dbContext.Bets.ToArrayAsync();
			_dbOdds = await _dbContext.Odds.ToArrayAsync();
		}

		private void ProcessSport(XmlSport xmlSport)
		{
			Sport sport = _dbSports.FirstOrDefault(s => s.Id == xmlSport.Id);

			if (sport is null)
			{
				sport = new(xmlSport.Id, xmlSport.Name, ProcessEvents(xmlSport.Events, xmlSport.Id));
				_dbContext.Sports.Add(sport);
				return;
			}

			sport.Id = xmlSport.Id;
			sport.Name = xmlSport.Name;
			sport.Events = ProcessEvents(xmlSport.Events, xmlSport.Id);
			_dbContext.Sports.Update(sport);
		}

		private List<Event> ProcessEvents(IEnumerable<XmlEvent> xmlEvents, int sportId)
		{
			List<Event> events = new();

			foreach (XmlEvent xmlEvent in xmlEvents)
			{
				Event @event = _dbEvents.FirstOrDefault(e => e.Id == xmlEvent.Id);

				if (@event is null)
				{
					@event = new(xmlEvent.Id, xmlEvent.Name, xmlEvent.IsLive, xmlEvent.CategoryID, sportId, ProcessMatches(xmlEvent.Matches, xmlEvent.Id));
					_dbContext.Events.Add(@event);
					events.Add(@event);
					continue;
				}

				@event.Id = xmlEvent.Id;
				@event.Name = xmlEvent.Name;
				@event.IsLive = xmlEvent.IsLive;
				@event.CategoryId = xmlEvent.CategoryID;
				@event.SportId = sportId;
				@event.Matches = ProcessMatches(xmlEvent.Matches, xmlEvent.Id);
				events.Add(@event);
			}

			return events;
		}

		private List<Match> ProcessMatches(IEnumerable<XmlMatch> xmlMatches, int eventId)
		{
			List<Match> matches = new();

			foreach (XmlMatch xmlMatch in xmlMatches)
			{
				if (xmlMatch.MatchType == MatchTypesEnum.OutRight)
					continue;

				Match match = _dbMatches.FirstOrDefault(m => m.Id == xmlMatch.Id);

				if (match is null)
				{
					match = new(xmlMatch.Id, xmlMatch.Name, xmlMatch.StartDate, xmlMatch.MatchType, eventId, ProcessBets(xmlMatch.Bets, xmlMatch.Id));
					_dbContext.Matches.Add(match);
					matches.Add(match);
					continue;
				}

				match.Id = xmlMatch.Id;
				match.Name = xmlMatch.Name;
				match.EventId = eventId;
				match.IsActive = true;
				match.Bets = ProcessBets(xmlMatch.Bets, xmlMatch.Id);
				matches.Add(match);

				WebSocketHelper<MatchUpdateMessage>.eventsQueue.Enqueue(new MatchUpdateMessage(
					new(xmlMatch.Id, xmlMatch.Name, xmlMatch.StartDate, xmlMatch.MatchType)));
			}

			return matches;
		}

		private List<Bet> ProcessBets(ICollection<XmlBet> xmlBets, int matchId)
		{
			List<Bet> bets = new();

			foreach (XmlBet xmlBet in xmlBets)
			{
				Bet bet = _dbBets.FirstOrDefault(m => m.Id == xmlBet.Id);

				if (bet is null)
				{
					bet = new(xmlBet.Id, xmlBet.Name, xmlBet.IsLive, matchId, ProcessOdds(xmlBet.Odds, xmlBet.Id));
					_dbContext.Bets.Add(bet);
					bets.Add(bet);
					continue;
				}

				bet.Id = xmlBet.Id;
				bet.Name = xmlBet.Name;
				bet.IsLive = xmlBet.IsLive;
				bet.IsActive = true;
				bet.MatchId = matchId;
				bet.Odds = ProcessOdds(xmlBet.Odds, xmlBet.Id);
				bets.Add(bet);

				WebSocketHelper<BetUpdateMessage>.eventsQueue.Enqueue(new BetUpdateMessage(
					new(xmlBet.Id, xmlBet.Name, xmlBet.IsLive)));
			}

			return bets;
		}

		private List<Odd> ProcessOdds(IEnumerable<XmlOdd> xmlOdds, int betId)
		{
			List<Odd> odds = new();

			foreach (XmlOdd xmlOdd in xmlOdds)
			{
				Odd odd = _dbOdds.FirstOrDefault(m => m.Id == xmlOdd.Id);

				if (odd is null)
				{
					odd = new(xmlOdd.Id, xmlOdd.Name, xmlOdd.Value, xmlOdd.SpecialBetValue, betId);
					_dbContext.Odds.Add(odd);
					odds.Add(odd);
					continue;
				}

				odd.Id = xmlOdd.Id;
				odd.Name = xmlOdd.Name;
				odd.Value = xmlOdd.Value;
				odd.SpecialBetValue = xmlOdd.SpecialBetValue;
				odd.IsActive = true;
				odd.BetId = betId;
				odds.Add(odd);

				WebSocketHelper<OddUpdateMessage>.eventsQueue.Enqueue(new OddUpdateMessage(
					new(xmlOdd.Id, xmlOdd.Name, xmlOdd.Value, odd.SpecialBetValue)));
			}

			return odds;
		}

		private void SetInactiveOdds(List<Odd> inactiveOdds)
		{
			foreach (Odd odd in inactiveOdds)
			{
				odd.IsActive = false;
				_dbContext.Odds.Add(odd);

				WebSocketHelper<InactiveOddUpdateMessage>.eventsQueue.Enqueue(new InactiveOddUpdateMessage(
					new(odd.Id, odd.Name)));
			}
		}

		private void SetInactiveBets(List<Bet> inactiveBets)
		{
			foreach (Bet bet in inactiveBets)
			{
				bet.IsActive = false;
				_dbContext.Bets.Add(bet);

				WebSocketHelper<InactiveBetUpdateMessage>.eventsQueue.Enqueue(new InactiveBetUpdateMessage(
					new(bet.Id, bet.Name)));
			}
		}

		private void SetInactiveMatches(List<Match> inactiveMatches)
		{
			foreach (Match match in inactiveMatches)
			{
				match.IsActive = false;
				_dbContext.Matches.Add(match);

				WebSocketHelper<InactiveMatchUpdateMessage>.eventsQueue.Enqueue(new InactiveMatchUpdateMessage(
					new(match.Id, match.Name, match.MatchType)));
			}
		}
	}
}