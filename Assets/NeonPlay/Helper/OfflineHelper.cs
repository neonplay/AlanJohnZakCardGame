using NeonPlay.Helper;
using System;

namespace NeonPlayHelper {

	public static class OfflineHelper {

		private const string KeyPrefix = "OfflineHelper";
		private const string OfflineEpochTimeKey = KeyPrefix + "OfflineEpochTime";
		private const string OfflineSteadyTimeKey = KeyPrefix + "OfflineSteadyTime";
		private const string OnlineTotalTimeKey = KeyPrefix + "OnlineTotalTime";

		private static bool activated;
		private static double onlineEpochTime;
		private static double onlineSteadyTime;

		public static double OfflineDuration { get; private set; }
		public static double MinimumOfflineDuration { get; set; } = 10.0 * 60.0;
		public static double MaximumOfflineDuration { get; set; } = 8.0 * 60.0 * 60.0;

		public static DateTime Epoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		public static double EpochTime => DateTime.UtcNow.Subtract(Epoch).TotalSeconds;
		public static double SteadyTime => NeonPlay.SteadyTime.Now;

		public static event Action<double> OfflineHasEnded;

		private static double OfflineEpochTime {
			get => SaveGameHelper.GetDouble(OfflineEpochTimeKey, EpochTime);
			set => SaveGameHelper.SetDouble(OfflineEpochTimeKey, value);
		}

		private static double OfflineSteadyTime {
			get => SaveGameHelper.GetDouble(OfflineSteadyTimeKey, SteadyTime);
			set => SaveGameHelper.SetDouble(OfflineSteadyTimeKey, value);
		}

		private static double OnlineTotalTime {
			get => SaveGameHelper.GetDouble(OnlineTotalTimeKey);
			set => SaveGameHelper.SetDouble(OnlineTotalTimeKey, value);
		}

		public static void Activate() {

			if (!activated) {

				activated = true;

				PauseHelper.Activate();
				PauseHelper.OnPause += OnPause;

				Online();
			}
		}

		private static void OnPause(bool paused) {

			if (paused) {

				double previousOnlineTotalTime = OnlineTotalTime;

				OfflineEpochTime = EpochTime;
				OfflineSteadyTime = SteadyTime;

				OnlineTotalTime += MathfHelper.Max(0.0, MathfHelper.Min(OfflineEpochTime - onlineEpochTime, OfflineSteadyTime - onlineSteadyTime));

				foreach (AnalyticsHelper.TotalPlayTime totalPlayTime in Enum.GetValues(typeof(AnalyticsHelper.TotalPlayTime))) {

					if ((previousOnlineTotalTime < (int)totalPlayTime) && (OnlineTotalTime >= (int)totalPlayTime)) {

						AnalyticsHelper.LogTotalPlayTime(totalPlayTime);
					}
				}

			} else {

				Online();
			}
		}

		private static void Online() {

			onlineEpochTime = EpochTime;
			onlineSteadyTime = SteadyTime;

			double epochTimeDuration = onlineEpochTime - OfflineEpochTime;
			double steadyTimeDuration = onlineSteadyTime - OfflineSteadyTime;
			double duration = MathfHelper.Max(0.0, epochTimeDuration);

			if ((steadyTimeDuration > 0.0) && (steadyTimeDuration < duration)) {

				duration = steadyTimeDuration;
			}

			OfflineDuration = MathfHelper.Min(duration, MaximumOfflineDuration);

			if ((OfflineDuration > 0.0) && (OfflineDuration >= MinimumOfflineDuration)) {

				OfflineHasEnded?.Invoke(OfflineDuration);
			}
		}
	}
}
