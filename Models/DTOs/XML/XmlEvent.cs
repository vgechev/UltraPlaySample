using System.Xml.Serialization;

namespace UltraPlaySample.Models.DTOs.XML
{
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
}