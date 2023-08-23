using System.Xml.Serialization;

namespace UltraPlaySample.Models.DTOs.XML
{
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