using System.Xml.Serialization;
using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Models.DTOs.XML
{
    public class XmlMatch
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("StartDate")]
		public DateTime StartDate { get; set; }

		[XmlAttribute("MatchType")]
		public MatchTypesEnum MatchType { get; set; }

		[XmlElement("Bet")]
		public XmlBet[] Bets { get; set; }
	}
}