using System;

namespace NeonPlayHelper {

	public static partial class AdvertHelper {

		public enum BannerPosition {
			Top,
			Bottom
		}

		private interface IPlatform {

			bool IsInitialised { get; }
			bool HasRewarded { get; }
			bool HasInterstitial { get; }
			bool HasBanner { get; }
			bool BannerVisible { get; }

			bool? GdprApplicable { get; }
			bool DoNotSell { set; }
			bool AgeConfirmed { set; }
			bool TrackingAllowed { set; }

			event Action RewardedAvailabilityHasChanged;
			event Action InterstitialAvailabilityHasChanged;
			event Action<AnalyticsHelper.RevenueData> AdvertRevenueHasPaid;

			bool Initialise(string bannerId, string interstitialId, string rewardedId, Action onComplete);
			bool RequestRewarded(Action onComplete, Action<string> onFailed);
			bool ShowRewarded(Action onComplete, Action<string> onFailed, Action<string, double> onReward);
			bool RequestInterstitial(Action onComplete, Action<string> onFailed);
			bool ShowInterstitial(Action onComplete, Action<string> onFailed);

			bool RequestBanner(BannerPosition position, Action onComplete, Action<string> onFailed);
			bool ShowBanner();
			bool HideBanner();
			bool SetBannerPosition(BannerPosition position);

			void ShowDebugger();
		}

		private static readonly IPlatform Platform;

		static AdvertHelper() {

			Platform = new AppLovinMaxPlatform();
		}

		public static bool IsInitialising { get; private set; }
		public static bool IsInitialised => Platform.IsInitialised;
		public static bool HasRewarded => Platform.HasRewarded;
		public static bool HasInterstitial => Platform.HasInterstitial;
		public static bool HasBanner => Platform.HasBanner;
		public static bool BannerVisible => Platform.BannerVisible;
		public static bool ShowingRewarded { get; private set; }
		public static bool ShowingInterstitial { get; private set; }

		public static event Action RewardedAvailabilityHasChanged;
		public static event Action InterstitialAvailabilityHasChanged;
		public static event Action<AnalyticsHelper.RevenueData> AdvertRevenueHasPaid;

		public static bool Initialise(string bannerId = "", string interstitialId = "", string rewardedId = "", Action onComplete = null) {

			ExecutionHelper.Activate();

			if (!PrivacyHelper.IsInitialised) {

				throw new InvalidOperationException("The PrivacyHelper must be initialised first");
			}

			if (IsInitialising || IsInitialised) {

				throw new InvalidOperationException("Already initialised");
			}

			NeonPlayConfiguration configuration = NeonPlayConfiguration.RuntimeInstance;

			if (configuration != null) {

				NeonPlayConfiguration.Advert.IdSet idSet = configuration.AdvertConfiguration.CurrentIdSet;

				AssignConfigurationId(ref bannerId, idSet.BannerId);
				AssignConfigurationId(ref interstitialId, idSet.InterstitialId);
				AssignConfigurationId(ref rewardedId, idSet.RewardedId);
			}

			IsInitialising = true;

			return Platform.Initialise(bannerId, interstitialId, rewardedId, () => {

				Platform.RewardedAvailabilityHasChanged += () => ExecutionHelper.EnqueueExecution(RewardedAvailabilityHasChanged);
				Platform.InterstitialAvailabilityHasChanged += () => ExecutionHelper.EnqueueExecution(InterstitialAvailabilityHasChanged);
				Platform.AdvertRevenueHasPaid += revenue => ExecutionHelper.EnqueueExecution(revenue, AdvertRevenueHasPaid);

				ExecutionHelper.EnqueueExecution(() => {

					IsInitialising = false;
					onComplete?.Invoke();
				});
			});

			void AssignConfigurationId(ref string id, string configurationId) {

				if (string.IsNullOrEmpty(id)) {

					id = configurationId;
				}
			}
		}

		public static bool? GdprApplicable => IsInitialised ? Platform.GdprApplicable : null;

		public static void UpdateConsent() {

			if (!IsInitialised) {

				throw new InvalidOperationException("Not yet initialised");
			}

			Platform.AgeConfirmed = PrivacyHelper.AgeConfirmed;
			Platform.TrackingAllowed = PrivacyHelper.TrackingAllowed;
			Platform.DoNotSell = PrivacyHelper.DoNotSell;
		}

		public static bool RequestRewarded(Action onComplete, Action<string> onFailed) {

			if (IsInitialised && !HasRewarded && !ShowingRewarded) {

				return Platform.RequestRewarded(() => ExecutionHelper.EnqueueExecution(onComplete), error => ExecutionHelper.EnqueueExecution(error, onFailed));
			}

			return false;
		}

		public static bool RequestInterstitial(Action onComplete, Action<string> onFailed) {

			if (IsInitialised && !HasInterstitial && !ShowingInterstitial) {

				return Platform.RequestInterstitial(() => ExecutionHelper.EnqueueExecution(onComplete), error => ExecutionHelper.EnqueueExecution(error, onFailed));
			}

			return false;
		}

		public static bool ShowRewarded(Action onComplete, Action<string> onFailed, Action<string, double> onReward = null) {

			if (IsInitialised && HasRewarded && !ShowingRewarded && !ShowingInterstitial) {

				if (Platform.ShowRewarded(() => ExecutionHelper.EnqueueExecution(() => {

					ShowingRewarded = false;
					onComplete?.Invoke();

				}), error => ExecutionHelper.EnqueueExecution(error, error => {

					ShowingRewarded = false;
					onFailed?.Invoke(error);

				}), (item, amount) => ExecutionHelper.EnqueueExecution(item, amount, onReward))) {

					ShowingRewarded = true;
					return true;
				}
			}

			return false;
		}

		public static bool ShowInterstitial(Action onComplete, Action<string> onFailed) {

			if (IsInitialised && HasInterstitial && !ShowingInterstitial && !ShowingRewarded) {

				if (Platform.ShowInterstitial(() => ExecutionHelper.EnqueueExecution(() => {

					ShowingInterstitial = false;
					onComplete?.Invoke();

				}), error => ExecutionHelper.EnqueueExecution(error, error => {

					ShowingInterstitial = false;
					onFailed?.Invoke(error);

				}))) {

					ShowingInterstitial = true;
					return true;
				}
			}

			return false;
		}

		public static bool RequestBanner(BannerPosition position, Action onComplete, Action<string> onFailed) {

			return IsInitialised && !HasBanner && Platform.RequestBanner(position, () => ExecutionHelper.EnqueueExecution(onComplete), error => ExecutionHelper.EnqueueExecution(error, onFailed));
		}

		public static bool ShowBanner() {

			return IsInitialised && HasBanner && Platform.ShowBanner();
		}

		public static bool HideBanner() {

			return IsInitialised && HasBanner && Platform.HideBanner();
		}

		public static void ShowDebugger() {

			if (IsInitialised) {

				Platform.ShowDebugger();
			}
		}
	}
}
