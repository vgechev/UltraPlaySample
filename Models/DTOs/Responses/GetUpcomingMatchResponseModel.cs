namespace UltraPlaySample.Models.DTOs.Responses
{
    public record GetUpcomingMatchResponseModel(string Name, DateTime StartDate, BetDto[] Bets);
}