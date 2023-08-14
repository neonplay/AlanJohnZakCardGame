using System;
using UnityEngine;

namespace NeonPlayHelper {

	public static class VersionHelper {

		public static string VersionString => "v" + Application.version + ApplicationHelper.BuildNumberSuffix + AbHelper.ModeHashSuffix;

		private static string osVersionString;

		public static string OsVersionString {
			get {
				if (osVersionString == null) {
#if UNITY_EDITOR
					osVersionString = string.Empty;
#elif UNITY_IOS
					osVersionString = UnityEngine.iOS.Device.systemVersion;
#elif UNITY_ANDROID
					try {

						using (var version = new AndroidJavaClass("android.os.Build$VERSION")) {

							osVersionString = version.GetStatic<string>("RELEASE");
						}

					} catch (Exception exception) {

						osVersionString = string.Empty;
					}
#else
					osVersionString = string.Empty;
#endif
				}

				return osVersionString;
			}
		}

		public static Version OsVersion {
			get {
				if (Version.TryParse(OsVersionString, out Version versionNumber)) {

					return versionNumber;
				}

				return new Version("0");
			}
		}
	}
}
