using System;

namespace NeonPlayHelper {

	public static partial class AdvertisingIdentifierHelper {

		private class StubPlatform : IPlatform {

			public bool RequestPermission(Action action) {

				return false;
			}

			public bool RequestIdentifier(Action<string, bool, string> onComplete) {

				return false;
			}
		}
	}
}
