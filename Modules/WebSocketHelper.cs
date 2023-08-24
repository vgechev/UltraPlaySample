namespace UltraPlaySample.Modules
{
	public static class WebSocketHelper
	{
		// TODO: Use explicit type instead of object, something like EventMessage<T>
		public static Queue<object> eventsQueue = new();
	}
}
