using System.Xml.Serialization;
using UltraPlaySample.Enums;

namespace UltraPlaySample.Models
{

	[XmlRoot("XmlSports")]
	public class XmlSports
	{
		[XmlElement("Sport")]
		public Sport[] Sports { get; set; }

		[XmlAttribute("CreateDate")]
		public DateTime CreateDate { get; set; }
	}

	public class Sport
	{
		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlElement("Event")]
		public Event[] Events { get; set; }
	}

	public class Event
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
		public Match[] Matches { get; set; }
	}

	public class Match
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
		public Bet[] Bets { get; set; }
	}

	public class Bet
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("ID")]
		public int Id { get; set; }

		public bool IsLive { get; set; }

		[XmlElement("Odd")]
		public Odd[] Odds { get; set; }
	}

	public class Odd
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