using System;
using System.Diagnostics;
using UnityEngine;

namespace NeonPlay {

	public class AdvertisingIdentifier : MonoBehaviour {

		private Action<string, bool, string> HasCompleted;

		public static bool Request(Action<string, bool, string> onComplete) {

#if UNITY_ANDROID && !UNITY_EDITOR && USING_EXTERNALDEPENDENCYMANAGER
			GameObject gameObject = new GameObject("AdvertisingIdentifier");
			AdvertisingIdentifier advertisingIdentifier = gameObject.AddComponent<AdvertisingIdentifier>();
			DontDestroyOnLoad(gameObject);

			advertisingIdentifier.HasCompleted += onComplete;

			return true;
#else
			return false;
#endif
		}

		private void Start() {

			Log("Start");

			AndroidJavaClass advertisingIdentifierActivity = new AndroidJavaClass("com.neonplay.advertisingidentifier.AdvertisingIdentifier");

			advertisingIdentifierActivity.CallStatic("Request");
		}

		private void Complete(string id, bool tracking, string error) {

			HasCompleted?.Invoke(id, tracking, error);

			Destroy(gameObject);
		}

		private void OnLimited(string id) {

			Log("OnLimited: ID: " + id);

			Complete(id, false, string.Empty);
		}

		private void OnAvailable(string id) {

			Log("OnAvailable: ID: " + id);

			Complete(id, true, string.Empty);
		}

		private void OnUnavailable(string error) {

			Log("OnUnavailable: Error: " + error);

			Complete(string.Empty, false, string.Empty);
		}

#if ENABLE_DEBUG_LOG
		[DebugLogging.Category]
		public static readonly string LogCategory = typeof(AdvertisingIdentifier).FullName;
#endif

		[Conditional("ENABLE_DEBUG_LOG")]
		private void Log(string message) {

#if ENABLE_DEBUG_LOG
			DebugLogging.Log(LogCategory, message);
#endif
		}
	}
}
