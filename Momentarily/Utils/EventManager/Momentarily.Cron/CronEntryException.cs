using System;
namespace Momentarily.Cron
{
	public class CronEntryException : Exception
	{
		public CronEntryException(string message)
			: base(message)
		{
		}
	}
}