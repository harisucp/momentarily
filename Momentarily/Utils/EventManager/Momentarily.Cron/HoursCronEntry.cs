namespace Momentarily.Cron
{
	public class HoursCronEntry : CronEntryBase
	{
		public HoursCronEntry(string expression)
		{
			Initialize(expression, 0, 23);
		}
	}
}