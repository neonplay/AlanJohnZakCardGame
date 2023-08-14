using Facebook.Unity;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_IOS
namespace AudienceNetwork {

	public static class AdSettings {

		[DllImport("__Internal")]
		private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

		public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled) {

			FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
		}
	}
}
#endif

namespace NeonPlayHelper {

	public static class PrivacyHelper {

		private const string KeyPrefix = "NeonPlayPrivacyHelper";
		private const string AgeConfirmedKey = KeyPrefix + "AgeConfirmed";
		private const string AcceptedTrackingKey = KeyPrefix + "AcceptedTracking";
		private const string AcceptedAnalyticsKey = KeyPrefix + "AcceptedAnalytics";		
		private const string DoNotSellKey = KeyPrefix + "DoNotSell";
		private const string GdprCompleteKey = KeyPrefix + "GdprComplete";

		private static bool? gdprApplicable;

		public static bool IsInitialised { get; private set; }

		public static bool GdprApplicable {
			get => gdprApplicable ?? RegionHelper.GdprRegion;
			set => gdprApplicable = value;
		}

		public static bool CcpaApplicable => !GdprApplicable && RegionHelper.CcpaRegion;

		public static bool GdprComplete {
			get => SaveGameHelper.GetBoolean(GdprCompleteKey, false);
			set => SaveGameHelper.SetBoolean(GdprCompleteKey, value);
		}

		public static bool GdprAgeConfirmed {
			get => SaveGameHelper.GetBoolean(AgeConfirmedKey, false);
			set => SaveGameHelper.SetBoolean(AgeConfirmedKey, value);
		}

		public static bool GdprAcceptedTracking {
			get => SaveGameHelper.GetBoolean(AcceptedTrackingKey, false);
			set => SaveGameHelper.SetBoolean(AcceptedTrackingKey, value);
		}

		public static bool GdprAcceptedAnalytics {
			get => SaveGameHelper.GetBoolean(AcceptedAnalyticsKey, false);
			set => SaveGameHelper.SetBoolean(AcceptedAnalyticsKey, value);
		}

		public static bool CcpaDoNotSell {
			get => SaveGameHelper.GetBoolean(DoNotSellKey, false);
			set => SaveGameHelper.SetBoolean(DoNotSellKey, value);
		}

		public static bool AgeConfirmed => !GdprApplicable || GdprAgeConfirmed;
		public static bool TrackingAllowed => !GdprApplicable || GdprAcceptedTracking;
		public static bool AnalyticsAllowed => !GdprApplicable || GdprAcceptedAnalytics;
		public static bool DoNotSell => CcpaApplicable && CcpaDoNotSell;

		public static void Initialise() {

			if (AdvertHelper.IsInitialising || AdvertHelper.IsInitialised) {

				throw new InvalidOperationException("Initialise must be called before the AdvertHelper is initialised");
			}

			if (IsInitialised) {

				throw new InvalidOperationException("Already initialised");
			}

			EnableFacebookTracking(TrackingAllowed);

			IsInitialised = true;
		}

		private static void EnableFacebookTracking(bool enable) {

			if (FB.IsInitialized) {

				FB.Mobile.SetAdvertiserTrackingEnabled(enable);

			} else {

				Debug.LogError("Facebook is not yet initialised. If this isn't done at all then ignore, otherwise move the initialisation to before PrivacyHelper.Initialise()");
			}

#if !UNITY_EDITOR && UNITY_IOS
			AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(enable);
#endif
		}
	}
}
