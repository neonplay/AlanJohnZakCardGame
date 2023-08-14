using Balaso;
using System;

namespace NeonPlayHelper {

	public static partial class AdvertisingIdentifierHelper {

		private class IosPlatform : IPlatform {

			private bool requestingTracking;

			private readonly Version AppTransparencyTrackingOsVersion = new Version("14.5");
			private bool PermissionRequired => VersionHelper.OsVersion >= AppTransparencyTrackingOsVersion;

			public bool RequestPermission(Action onComplete) {

				if (PermissionRequired) {

					AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;

					if (!requestingTracking && (currentStatus != AppTrackingTransparency.AuthorizationStatus.AUTHORIZED)) {

						requestingTracking = true;
						AppTrackingTransparency.OnAuthorizationRequestDone += OnDone;
						AppTrackingTransparency.RequestTrackingAuthorization();

						return true;

						void OnDone(AppTrackingTransparency.AuthorizationStatus status) {

							AppTrackingTransparency.OnAuthorizationRequestDone -= OnDone;
							requestingTracking = false;

							onComplete?.Invoke();
						}
					}
				}

				return false;
			}

			public bool RequestIdentifier(Action<string, bool, string> onComplete) {

				if (!requestingTracking) {

					AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;

					if (currentStatus == AppTrackingTransparency.AuthorizationStatus.AUTHORIZED) {

						string id = AppTrackingTransparency.IdentifierForAdvertising();

						onComplete(id, true, string.Empty);

					} else {

						onComplete(string.Empty, false, currentStatus.ToString());
					}

					return true;
				}

				return false;
			}
		}
	}
}
