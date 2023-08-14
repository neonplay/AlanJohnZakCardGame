using System;

namespace NeonPlayHelper {

	public static class DayHelper {

		public static DateTime LocalEpoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
		public static int CurrentDay => (int)DateTime.UtcNow.Subtract(LocalEpoch).TotalDays;
	}
}
