using System;

namespace odTimeTracker
{
	namespace Model
	{
		/// <summary>
		/// Enum with all available payment types for projects.
		/// </summary>
		public enum PaymentTypeEnum
		{
			HOURLY,
			DAILY,
			MONTHLY,
			CUSTOM,
			NONE
		}
	}
}
