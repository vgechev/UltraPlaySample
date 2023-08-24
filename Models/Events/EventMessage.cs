namespace UltraPlaySample.Models.Events
{
	public record EventMessage<T>(string EventType, T Payload);
}
