using System;
namespace AutoTest.Core.Notifiers
{
	public enum NotificationType
	{
		Information,
		Green,
		Yellow,
		Red
	}
	
	public interface ISendNotifications
	{
		void Notify(string msg, NotificationType type);
		bool IsSupported();
	}
}

