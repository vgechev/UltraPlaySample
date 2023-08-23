using UltraPlaySample.Models;

namespace UltraPlaySample.Services.Interfaces
{
	public interface IXmlDataService
	{
		public Task<XmlSports> FetchXmlData();
		public Task ProcessDataAndSaveToDatabase(XmlSport[] sports);
	}
}