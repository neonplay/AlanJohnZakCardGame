using System.Collections.Generic;
using UnityEngine;

namespace NeonPlayHelper {

	public static partial class AnalyticsHelper {

		public const string SingularApiKey = "neonplay_03571954";
		public const string SingularApiSecret = "e79530bb4c8e267b3622b89a5bccc316";

		private class Singular : IPlatform {

			private bool initialised;

			public Singular() {

				CreateSdkObject();
			}

			private void CreateSdkObject() {

				GameObject singularGameObject = new GameObject("SingularSDKObject");

				singularGameObject.SetActive(false);

				SingularSDK singularSdk = singularGameObject.AddComponent<SingularSDK>();

				singularSdk.SingularAPIKey = SingularApiKey;
				singularSdk.SingularAPISecret = SingularApiSecret;
				singularSdk.InitializeOnAwake = false;
#if UNITY_IOS
				singularSdk.SKANEnabled = true;
#endif

				singularGameObject.SetActive(true);
			}

			public void Activate() {

				if (!Application.isEditor) {

					if (!initialised) {

						SingularSDK.InitializeSingularSDK();
						SingularSDK.TrackingOptIn();

						initialised = true;
					}

					SingularSDK.ResumeAllTracking();
				}
			}

			public void Deactivate() {

				if (!Application.isEditor) {

					SingularSDK.StopAllTracking();
				}
			}

			public void SetAbMode(string experiment) {

			}

			public void LogDesignEvent(string eventString) {

			}

			public void LogDesignEvent(string eventString, float value) {

			}

			public void LogCustomEvent(string eventString, Dictionary<string, object> eventData) {

				switch (eventString) {
					case TotalPlayTimeEventName:

						object firstParameter;

						if (eventData.TryGetValue(FirstParameterKey, out firstParameter)) {

							if (firstParameter is TotalPlayTime totalPlayTime) {

								SingularSDK.Event("Played" + totalPlayTime);
							}
						}

						break;

					case TutorialCompleteEventName:

						SingularSDK.Event("TutorialComplete");
						break;
				}
			}

			public void LogAdvertRevenue(RevenueData revenueData) {

				SingularSDK.AdRevenue(new SingularAdData(revenueData.AdvertPlatform, revenueData.Currency, revenueData.Revenue).WithAdGroupId(revenueData.AdGroupId).WithAdGroupName(revenueData.AdGroupName).WithAdGroupType(revenueData.AdGroupType).WithAdPlacmentName(revenueData.PlacementName).WithAdType(revenueData.Type).WithAdUnitId(revenueData.AdUnitId).WithAdUnitName(revenueData.AdUnitName).WithImpressionId(revenueData.ImpressionId).WithNetworkName(revenueData.NetworkName).WithPlacementId(revenueData.PlacementId).WithPrecision(revenueData.Precision));
			}

#if UNITY_PURCHASING
			public void LogTransaction(UnityEngine.Purchasing.Product product, bool restored) {

				SingularSDK.InAppPurchase(product, null, restored);
			}
#endif
		}
	}
}
