using System;
using UnityEngine;

namespace NeonPlay {

	public static class SteadyTime {

		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		private static double GetSteadyTimeNow() {

			return Environment.TickCount / 1000.0;
		}
		#elif UNITY_IPHONE
		[System.Runtime.InteropServices.DllImport("__Internal")]
		private static extern double GetSteadyTimeNow();
		#elif UNITY_ANDROID
		private static AndroidJavaClass AndroidSystemClock;

		private static double GetSteadyTimeNow() {

			if (AndroidSystemClock == null) {

				AndroidSystemClock = new AndroidJavaClass("android.os.SystemClock");
			}

			return (double) AndroidSystemClock.CallStatic<long>("elapsedRealtime") / 1000.0;
		}
		#else
		#error Unexpected platform specified.
		#endif

		public static double Now => GetSteadyTimeNow();
	}
}
