using System.ComponentModel.DataAnnotations;
using UltraPlaySample.Common.Enums;

namespace UltraPlaySample.Data.Entities
{
	public class MatchType
	{
		[Key]
		public MatchTypesEnum Id { get; set; }
		public string Name { get; set; }

		public MatchType() { }
		public MatchType(MatchTypesEnum id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}