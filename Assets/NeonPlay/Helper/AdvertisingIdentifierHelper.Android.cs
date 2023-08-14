using NeonPlay;
using System;

namespace NeonPlayHelper {

	public static partial class AdvertisingIdentifierHelper {

		private class AndroidPlatform : IPlatform {

			public bool RequestPermission(Action onComplete) {

				return false;
			}

			public bool RequestIdentifier(Action<string, bool, string> onComplete) {

				return AdvertisingIdentifier.Request(onComplete);
			}
		}
	}
}
