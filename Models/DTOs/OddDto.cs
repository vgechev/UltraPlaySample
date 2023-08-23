using UltraPlaySample.Data.Entities;

namespace UltraPlaySample.Models.DTOs
{
    public record OddDto(string Name, decimal? SpecialBetValue, decimal Value)
    {
        public static OddDto GetDtoFromEntity(Odd entity) => new(entity.Name, entity.SpecialBetValue, entity.Value);
    }
}