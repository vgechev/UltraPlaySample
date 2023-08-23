using System.Xml.Serialization;

namespace UltraPlaySample.Models.DTOs.XML
{

	[XmlRoot("XmlSports")]
	public class XmlSports
	{
		[XmlElement("Sport")]
		public XmlSport[] Sports { get; set; }

		[XmlAttribute("CreateDate")]
		public DateTime CreateDate { get; set; }
	}
}