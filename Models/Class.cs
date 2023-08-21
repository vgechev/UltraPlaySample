using System.Xml.Serialization;
using UltraPlaySample.Enums;

namespace UltraPlaySample.Models
{

	[XmlRoot("XmlSports")]
	public class XmlSports
	{
		[XmlElement("Sport")]
		public XmlSport[] Sports { get; set; }

		[XmlAttribute("CreateDate")]
		public DateTime CreateDate { get; set; }
	}

	public class XmlSport
	{
		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlElement("Event")]
		public XmlEvent[] Events { get; set; }
	}

	public class XmlEvent
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("IsLive")]
		public bool IsLive { get; set; }

		[XmlAttribute("CategoryID")]
		public int CategoryID { get; set; }

		[XmlElement("Match")]
		public XmlMatch[] Matches { get; set; }
	}

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

	public class XmlBet
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("ID")]
		public int Id { get; set; }

		public bool IsLive { get; set; }

		[XmlElement("Odd")]
		public XmlOdd[] Odds { get; set; }
	}

	public class XmlOdd
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("Value")]
		public decimal Value { get; set; }

		[XmlAttribute("SpecialBetValue")]
		public decimal SpecialBetValue { get; set; }
	}
}