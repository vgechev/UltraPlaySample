using UltraPlaySample.Models;

namespace UltraPlaySample.Services.Interfaces
{
    public interface IXmlDataService
    {
        public Task<XmlSports> FetchXmlData();
        public Task ProcessDataAndSaveToDatabase(Models.Sport[] sports);
    }
}