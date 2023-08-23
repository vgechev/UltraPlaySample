using System.Xml.Serialization;

namespace UltraPlaySample.Models.DTOs.XML
{
	public class XmlSport
	{
		[XmlAttribute("ID")]
		public int Id { get; set; }

		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlElement("Event")]
		public XmlEvent[] Events { get; set; }
	}
}