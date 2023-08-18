using UltraPlaySample.Models;

namespace UltraPlaySample.Services
{
	public interface IXmlDataService
	{
		public Task<XmlSports> FetchXmlData();
		public Task ProcessDataAndSaveToDatabase(Models.Sport[] sports);
	}
}