using System.Runtime.InteropServices;
using UnityEngine;

namespace NeonPlayHelper {

	public static class ApplicationHelper {

#if !UNITY_EDITOR
#if UNITY_IOS
		[DllImport("__Internal")]
		extern static private string AppInfoGetBuildNumber();
#elif UNITY_ANDROID
		public static string AppInfoGetBuildNumber() {

			AndroidJavaClass classUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = classUnity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);
			AndroidJavaClass classPackageInfoCompat = new AndroidJavaClass("androidx.core.content.pm.PackageInfoCompat");
			long longVersionCode = classPackageInfoCompat.CallStatic<long>("getLongVersionCode", packageInfo);

			longVersionCode &= 0xffffffff;

			return longVersionCode.ToString();
		}
#endif
#endif

		private static string buildNumber;

		public static string BuildNumber {
			get {
				if (buildNumber == null) {
#if UNITY_EDITOR
					buildNumber = UnityEditor.PlayerSettings.iOS.buildNumber;
#elif (UNITY_IOS || UNITY_ANDROID)
					try {

						buildNumber = AppInfoGetBuildNumber();

					} catch {

						buildNumber = string.Empty;
					}
#else
					buildNumber = string.Empty;
#endif
				}

				return buildNumber;
			}
		}

		public static string BuildNumberSuffix => string.IsNullOrEmpty(BuildNumber) ? string.Empty : ("." + BuildNumber);
	}
}
