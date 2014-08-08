using System;
namespace AutoTest.Core.Notifiers
{
	public class NullNotifier : ISendNotifications
	{
		#region ISendNotifications implementation
		public void Notify(string msg, NotificationType type)
		{
		}

		public bool IsSupported()
		{
			return true;
		}
		#endregion
	}
}

