using System;
using System.Collections.Generic;

namespace NeonPlayHelper {

    public static partial class AnalyticsHelper {

		public enum AdvertType {
			Banner,
			Interstitial,
			VideoReward,
			OfferWall
		}

		public enum TotalPlayTime {

			FiveMinutes = 5 * 60
		}

		public const string FirstParameterKey = "First";
		public const string SecondParameterKey = "Second";
		public const string ThirdParameterKey = "Third";
		public const string TotalPlayTimeEventName = "TotalPlayTime";
		public const string TutorialCompleteEventName = "TutorialComplete";

		public class RevenueData {

			public readonly AdvertType AdvertType;
			public readonly string AdvertPlatform;
			public readonly string Currency;
			public readonly double Revenue;

			public RevenueData(AdvertType advertType, string advertPlatform, string currency, double revenue) {

				AdvertType = advertType;
				AdvertPlatform = advertPlatform;
				Currency = currency;
				Revenue = revenue;
			}

			public string NetworkName { get; set; }
			public string Type { get; set; }
			public string ImpressionId { get; set; }
			public string PlacementId { get; set; }
			public string PlacementName { get; set; }
			public string AdUnitId { get; set; }
			public string AdUnitName { get; set; }
			public string AdGroupType { get; set; }
			public string AdGroupId { get; set; }
			public string AdGroupName { get; set; }
			public string AdGroupPriority { get; set; }
			public string Precision { get; set; }
		}

		public class TransactionData {

#if UNITY_PURCHASING
			public readonly UnityEngine.Purchasing.Product Product;
			public readonly bool Restored;

			public TransactionData(UnityEngine.Purchasing.Product product, bool restored) {

				Product = product;
				Restored = restored;
			}
#endif
		}

		private interface IPlatform {

			void Activate();
			void Deactivate();
			void SetAbMode(string experiment);
			void LogAdvertRevenue(RevenueData revenueData);
			void LogDesignEvent(string eventString);
			void LogDesignEvent(string eventString, float value);
			void LogCustomEvent(string eventString, Dictionary<string, object> eventData);

#if UNITY_PURCHASING
			void LogTransaction(UnityEngine.Purchasing.Product product, bool restored);
#endif
		}

		private class DesignEventData {

			public readonly string Name;
			public readonly float? Value;

			public DesignEventData(string name, float? value) {

				Name = name;
				Value = value;
			}
		}

		private class CustomEventData {

			public readonly string Name;
			public readonly Dictionary<string, object> Data;

			public CustomEventData(string name, Dictionary<string, object> data) {

				Name = name;
				Data = data;
			}
		}

		private static bool initialised;
		private static bool active;

		private static List<IPlatform> platformList;
		private static List<RevenueData> cachedRevenueList;
		private static List<TransactionData> cachedTransactionList;
		private static List<DesignEventData> cachedDesignEventList;
		private static List<CustomEventData> cachedCustomEventList;

		public static void Initialise() {

			if (!initialised) {

				platformList = new List<IPlatform>();
				platformList.Add(new Singular());
				platformList.Add(new Firebase());
				platformList.Add(new GameAnalytics());

				cachedRevenueList = new List<RevenueData>();
				cachedTransactionList = new List<TransactionData>();
				cachedDesignEventList = new List<DesignEventData>();
				cachedCustomEventList = new List<CustomEventData>();

				initialised = true;
			}
		}

		public static void UpdateConsent() {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (PrivacyHelper.AnalyticsAllowed) {

				Activate();

			} else {

				Deactivate();
			}
		}

		public static void Activate() {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (!active) {

				active = true;

				platformList.ForEach(platform => platform.Activate());

				cachedRevenueList.ForEach(revenueData => LogAdvertRevenue(revenueData));
				cachedRevenueList.Clear();

				cachedTransactionList.ForEach(transactionData => LogTransaction(transactionData));
				cachedTransactionList.Clear();

				cachedDesignEventList.ForEach(designEvent => LogDesignEvent(designEvent));
				cachedDesignEventList.Clear();

				cachedCustomEventList.ForEach(customEvent => LogCustomEvent(customEvent));
				cachedCustomEventList.Clear();
			}
		}

		public static void Deactivate() {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (active) {

				active = false;

				platformList.ForEach(platform => platform.Deactivate());
			}
		}

		public static void SetAbMode(string experiment) {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			platformList.ForEach(platform => platform.SetAbMode(experiment));
		}

		public static void LogDesignEvent(params string[] parameterList) {

			LogDesignEvent(parameterList, null);
		}

		public static void LogDesignEvent(float value, params string[] parameterList) {

			LogDesignEvent(parameterList, value);
		}

		private static void LogDesignEvent(string[] parameterList, float? value) {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			string name = string.Join(":", string.Join(":", Array.ConvertAll(string.Join(":", parameterList).Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries), item => item.Trim())).Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries));

			if (active) {

				LogDesignEvent(name, value);

			} else {

				cachedDesignEventList.Add(new DesignEventData(name, value));
			}
		}

		private static void LogDesignEvent(string name, float? value) {

			if (value.HasValue) {

				platformList.ForEach(platform => platform.LogDesignEvent(name, value.Value));

			} else {

				platformList.ForEach(platform => platform.LogDesignEvent(name));
			}
		}

		private static void LogDesignEvent(DesignEventData designEventData) {

			LogDesignEvent(designEventData.Name, designEventData.Value);
		}

		public static void LogAdvertRevenue(RevenueData revenueData) {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (active) {

				platformList.ForEach(platform => platform.LogAdvertRevenue(revenueData));

			} else {

				cachedRevenueList.Add(revenueData);
			}
		}

		private static void LogCustomEvent(CustomEventData customEventData) {

			LogCustomEvent(customEventData.Name, customEventData.Data);
		}

		private static void LogCustomEvent(string eventName, Dictionary<string, object> eventData = null) {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (active) {

				platformList.ForEach(platform => platform.LogCustomEvent(eventName, eventData));

			} else {

				cachedCustomEventList.Add(new CustomEventData(eventName, eventData));
			}
		}

		public static void LogTotalPlayTime(TotalPlayTime totalPlayTime) {

			Dictionary<string, object> eventData = new Dictionary<string, object>();

			eventData[FirstParameterKey] = totalPlayTime;

			LogCustomEvent(TotalPlayTimeEventName, eventData);
		}

		public static void LogTutorialComplete() {

			LogCustomEvent(TutorialCompleteEventName);
		}

		public static void LogTransaction(TransactionData transactionData) {

#if UNITY_PURCHASING
			LogTransaction(transactionData.Product, transactionData.Restored);
#endif
		}

#if UNITY_PURCHASING
		public static void LogTransaction(UnityEngine.Purchasing.Product product, bool restored) {

			if (!initialised) {

				throw new InvalidOperationException("Call Initialise() first");
			}

			if (active) {

				platformList.ForEach(platform => platform.LogTransaction(product, restored));

			} else {

				cachedTransactionList.Add(new TransactionData(product, restored));
			}
		}
#endif
	}
}
