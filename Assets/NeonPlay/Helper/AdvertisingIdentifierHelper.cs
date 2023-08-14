using System;

namespace NeonPlayHelper {

	public static partial class AdvertisingIdentifierHelper {

		private interface IPlatform {

			bool RequestPermission(Action onComplete);
			bool RequestIdentifier(Action<string, bool, string> onComplete);
		}

		private static readonly IPlatform Platform;

		static AdvertisingIdentifierHelper() {

#if UNITY_EDITOR
			Platform = new StubPlatform();
#elif UNITY_IOS
			Platform = new IosPlatform();
#elif UNITY_ANDROID
			Platform = new AndroidPlatform();
#else
			Platform = new StubPlatform();
#endif
		}

		public static bool RequestPermission(Action onComplete) {

			ExecutionHelper.Activate();

			return Platform.RequestPermission(() => ExecutionHelper.EnqueueExecution(onComplete));
		}

		public static bool RequestIdentifier(Action<string, bool, string> onComplete) {

			ExecutionHelper.Activate();

			return Platform.RequestIdentifier((id, enabled, error) => ExecutionHelper.EnqueueExecution(id, enabled, error, onComplete));
		}
	}
}
