using UltraPlaySample.Models.Events;

namespace UltraPlaySample.Modules
{
	public static class WebSocketHelper<T>
	{
		public static Queue<EventMessage<T>> eventsQueue = new();
	}
}