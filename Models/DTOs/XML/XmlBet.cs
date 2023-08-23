using System.Xml.Serialization;

namespace UltraPlaySample.Models.DTOs.XML
{
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
}