using NeonPlay.Json;
using System.Collections.Generic;

namespace NeonPlayHelper {

	public static partial class AnalyticsHelper {

		private class GameAnalytics : IPlatform {

			public void Activate() {

				GameAnalyticsSDK.GameAnalytics.SetEnabledEventSubmission(true);
			}

			public void Deactivate() {

				GameAnalyticsSDK.GameAnalytics.SetEnabledEventSubmission(false);
			}

			public void LogAdvertRevenue(RevenueData revenueData) {

			}

			public void LogDesignEvent(string eventString) {

				GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventString);
			}

			public void LogDesignEvent(string eventString, float value) {

				GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventString, value);
			}

			public void SetAbMode(string experiment) {

				GameAnalyticsSDK.GameAnalytics.SetCustomDimension01(experiment);
			}

			public void LogCustomEvent(string eventString, Dictionary<string, object> eventData) {

			}

#if UNITY_PURCHASING
			public void LogTransaction(UnityEngine.Purchasing.Product product, bool restored) {

				string transactionId = product.transactionID;
				string productId = product.definition.id;
				decimal price = product.metadata.localizedPrice;
				string currency = product.metadata.isoCurrencyCode;
				int minorUnitPrice = (int)(price * 100);

#if UNITY_IOS
				GameAnalyticsSDK.GameAnalytics.NewBusinessEventIOSAutoFetchReceipt(currency, minorUnitPrice, "default", productId, "default");
#elif UNITY_ANDROID
				string receipt = product.receipt;
				string signature = null;

				if (!string.IsNullOrEmpty(receipt)) {

					Dictionary<string, string> values = JsonDotNetHelper.DeserialiseObject<Dictionary<string, string>>(receipt);

					if (values != null) {

						values.TryGetValue("signature", out signature);
					}
				}

				GameAnalyticsSDK.GameAnalytics.NewBusinessEventGooglePlay(currency, minorUnitPrice, "default", productId, "default", receipt, signature);
#endif
				GameAnalyticsSDK.GameAnalytics.NewDesignEvent(productId);
			}
#endif
		}
	}
}
