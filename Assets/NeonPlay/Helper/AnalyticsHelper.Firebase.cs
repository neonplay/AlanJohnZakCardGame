using Firebase;
using Firebase.Analytics;
using System.Collections.Generic;

namespace NeonPlayHelper {

	public static partial class AnalyticsHelper {

		private class Firebase : IPlatform {

			private FirebaseApp firebaseApp;
			private List<RevenueData> delayedActivationRevenueList;
			private bool delayedActivation;

			private bool Ready => firebaseApp != null;

			public Firebase() {

				delayedActivationRevenueList = new List<RevenueData>();

				FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

					ExecutionHelper.EnqueueExecution(() => {

						DependencyStatus dependencyStatus = task.Result;

						if (dependencyStatus == DependencyStatus.Available) {

							firebaseApp = FirebaseApp.DefaultInstance;

							if (delayedActivation) {

								Activate();
							}
						}
					});
				});
			}

			public void Activate() {

				if (Ready) {

					FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

					if (delayedActivationRevenueList != null) {

						foreach (RevenueData revenueData in delayedActivationRevenueList) {

							LogAdvertRevenue(revenueData);
						}

						delayedActivationRevenueList = null;
					}

				} else {

					delayedActivation = true;
				}
			}

			public void Deactivate() {

				if (Ready) {

					FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
				}

				delayedActivation = false;
			}

			public void SetAbMode(string experiment) {

			}

			public void LogDesignEvent(string eventString) {

			}

			public void LogDesignEvent(string eventString, float value) {

			}

			public void LogCustomEvent(string eventString, Dictionary<string, object> eventData) {

			}

			public void LogAdvertRevenue(RevenueData revenueData) {

				if (Ready) {

					Parameter[] parameterList = {
						new Parameter("ad_platform", revenueData.AdvertPlatform),
						new Parameter("ad_source", revenueData.NetworkName),
						new Parameter("ad_unit_name", revenueData.Type),
						new Parameter("ad_format", revenueData.AdUnitName),
						new Parameter("currency", revenueData.Currency),
						new Parameter("value", revenueData.Revenue)
					};

					FirebaseAnalytics.LogEvent("ad_impression", parameterList);
					FirebaseAnalytics.LogEvent("np_ad_impression", parameterList);

				} else if (delayedActivationRevenueList != null) {

					delayedActivationRevenueList.Add(revenueData);
				}
			}

#if UNITY_PURCHASING
			public void LogTransaction(UnityEngine.Purchasing.Product product, bool restored) {

			}
#endif
		}
	}
}
