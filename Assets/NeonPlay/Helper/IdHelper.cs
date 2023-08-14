using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace NeonPlayHelper {

	public static class IdHelper {

		private const string PlayerIdKey = "PlayerIdHelper_PlayerId";
		private const string DeepLinkPrefix = "np";

		public static string PlayerId => SaveGameHelper.GetOrSetString(PlayerIdKey, DeviceId);

		private static AndroidJavaObject CurrentActivity {
			get {
#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE
				return null;
#elif UNITY_ANDROID
				AndroidJavaClass classUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

				return classUnity.GetStatic<AndroidJavaObject>("currentActivity");
#endif
			}
		}

		public static string DeviceId {
			get {
#if UNITY_EDITOR || UNITY_STANDALONE
				const string EditorKey = "Editor_DeviceID";

				if (!PlayerPrefs.HasKey(EditorKey)) {

					PlayerPrefs.SetString(EditorKey, Guid.NewGuid().ToString());
				}

				return PlayerPrefs.GetString(EditorKey);
#elif UNITY_IOS
				return UnityEngine.iOS.Device.vendorIdentifier;
#elif UNITY_ANDROID
				AndroidJavaObject objectActivity = CurrentActivity;
				AndroidJavaObject objectResolver = objectActivity.Call<AndroidJavaObject>("getContentResolver");
				AndroidJavaClass classSecure = new AndroidJavaClass("android.provider.Settings$Secure");

				string androidId = classSecure.CallStatic<string>("getString", objectResolver, "android_id");

				Debug.Log("Android ID: " + androidId);

				return androidId;
#endif
			}
		}

		public static string DeepLinkId {
			get {
				using (MD5 md5 = MD5.Create()) {

					byte[] md5Bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(Application.identifier));

					return DeepLinkPrefix + BitConverter.ToString(md5Bytes).Replace("-", "").Substring(0, 6).ToLower();
				}
			}
		}
	}
}
