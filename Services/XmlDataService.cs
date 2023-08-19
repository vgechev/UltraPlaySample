using System.Text;
using System.Xml.Serialization;
using UltraPlaySample.Models;
using UltraPlaySample.Services.Interfaces;

namespace UltraPlaySample.Services
{
    public class XmlDataService : IXmlDataService
	{
		private readonly AppDbContext dbContext;
		private readonly ILogger<XmlDataService> logger;
		private readonly IHttpClientFactory httpClientFactory;

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

		public async Task ProcessDataAndSaveToDatabase(Models.Sport[] sports)
		{
			// TODO: Get ids for each type with a single query and loop through each xml type and check if the Id exists in the database.
			// If yes, then update, otherwise insert.
			// The downside is - we will have a loop for every relation (currently 5), imagine if we add more
			// Maybe think of something else?
			await ClearDatabase();

			Sport[] sportEntities = sports.Select(xmlSport => new Sport
			{
				Id = xmlSport.Id,
				Name = xmlSport.Name,
				Events = xmlSport.Events.Select(xmlEvent => new Event
				{
					Id = xmlEvent.Id,
					Name = xmlEvent.Name,
					IsLive = xmlEvent.IsLive,
					CategoryId = xmlEvent.CategoryID,
					Matches = xmlEvent.Matches.Select(xmlMatch => new Match
					{
						Id = xmlMatch.Id,
						Name = xmlMatch.Name,
						StartDate = xmlMatch.StartDate,
						MatchType = xmlMatch.MatchType,
						Bets = xmlMatch.Bets.Select(xmlBet => new Bet
						{
							Id = xmlBet.Id,
							Name = xmlBet.Name,
							IsLive = xmlBet.IsLive,
							Odds = xmlBet.Odds.Select(xmlOdd => new Odd
							{
								Id = xmlOdd.Id,
								Name = xmlOdd.Name,
								Value = xmlOdd.Value,
								SpecialBetValue = xmlOdd.SpecialBetValue
							}).ToArray(),
						}).ToArray()
					}).ToArray()
				}).ToArray()
			}).ToArray();

			dbContext.Sports.AddRange(sportEntities);
			await dbContext.SaveChangesAsync();
		}

		private async Task ClearDatabase()
		{
			dbContext.Sports.RemoveRange(dbContext.Sports);
			dbContext.Events.RemoveRange(dbContext.Events);
			dbContext.Matches.RemoveRange(dbContext.Matches);
			dbContext.Bets.RemoveRange(dbContext.Bets);
			dbContext.Odds.RemoveRange(dbContext.Odds);

			await dbContext.SaveChangesAsync();
		}
	}
}