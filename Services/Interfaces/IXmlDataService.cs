using UltraPlaySample.Models.DTOs.XML;

namespace UltraPlaySample.Services.Interfaces
{
    public interface IXmlDataService
	{
		public Task<XmlSports> FetchXmlData();
		public Task ProcessDataAndSaveToDatabase(XmlSport[] sports);
	}
}