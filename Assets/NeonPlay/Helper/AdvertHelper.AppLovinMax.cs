using System;

namespace NeonPlayHelper {

	public static partial class AdvertHelper {

		public const string AppLovinMaxSdkKey = "7vBbt_yBVtOMgdysZEUNiA0xsckXS5ZZ_m9sl62F-Qbe2TUcnz_U8_Q-b9W3E4cDrn4Zu6qsWQUZz-BlyAD49d";

		private class AppLovinMaxPlatform : IPlatform {

			private string bannerAdUnit;
			private string interstitialAdUnit;
			private string rewardedAdUnit;
			private MaxSdkBase.SdkConfiguration sdkConfiguration;

			public event Action RewardedAvailabilityHasChanged;
			public event Action InterstitialAvailabilityHasChanged;
			public event Action<AnalyticsHelper.RevenueData> AdvertRevenueHasPaid;

			private bool SupportsRewarded => !string.IsNullOrEmpty(rewardedAdUnit);
			private bool SupportsInterstitial => !string.IsNullOrEmpty(interstitialAdUnit);
			private bool SupportsBanners => !string.IsNullOrEmpty(bannerAdUnit);

			public bool IsInitialised => sdkConfiguration != null;
			public bool HasRewarded => SupportsRewarded && MaxSdk.IsRewardedAdReady(rewardedAdUnit);
			public bool HasInterstitial => SupportsInterstitial && MaxSdk.IsInterstitialReady(interstitialAdUnit);
			public bool HasBanner => SupportsBanners && bannerLoaded;
			public bool BannerVisible => HasBanner && bannerShown;

			public bool? GdprApplicable {
				get {
					if (!IsInitialised) {

						throw new InvalidOperationException("Not yet initialised");
					}

					switch (sdkConfiguration.ConsentDialogState) {

						case MaxSdkBase.ConsentDialogState.Applies:
							return true;
						case MaxSdkBase.ConsentDialogState.DoesNotApply:
							return false;
						default:
							return null;
					}
				}
			}

			public bool DoNotSell {
				get => MaxSdk.IsDoNotSell();
				set => MaxSdk.SetDoNotSell(value);
			}

			public bool AgeConfirmed {
				get => !MaxSdk.IsAgeRestrictedUser();
				set => MaxSdk.SetIsAgeRestrictedUser(!value);
			}

			public bool TrackingAllowed {
				get => MaxSdk.HasUserConsent();
				set => MaxSdk.SetHasUserConsent(value);
			}

			private bool loadingBanner;
			private bool bannerLoaded;
			private bool bannerShown;
			private bool? hadRewarded;
			private bool? hadInterstitial;

			public bool Initialise(string bannerId, string interstitialId, string rewardedId, Action onComplete) {

				bannerAdUnit = bannerId;
				interstitialAdUnit = interstitialId;
				rewardedAdUnit = rewardedId;

				RegisterEvents();

				MaxSdkCallbacks.OnSdkInitializedEvent += OnInitialised;
				MaxSdk.SetSdkKey(AppLovinMaxSdkKey);
				MaxSdk.InitializeSdk();

				return true;

				void OnInitialised(MaxSdkBase.SdkConfiguration configuration) {

					MaxSdkCallbacks.OnSdkInitializedEvent -= OnInitialised;

					sdkConfiguration = configuration;
					onComplete?.Invoke();
				}
			}

			private void RegisterEvents() {

				MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (unitId, adInfo) => CheckRewardedAvailabilityChange();
				MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (unitId, errorInfo) => CheckRewardedAvailabilityChange();
				MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (unitId, adInfo) => CheckRewardedAvailabilityChange();
				MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (unitId, errorInfo, adInfo) => CheckRewardedAvailabilityChange();
				MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (unitId, adInfo) => RevenuePaid(AnalyticsHelper.AdvertType.VideoReward, unitId, adInfo);

				void CheckRewardedAvailabilityChange() {

					if (HasRewarded != hadRewarded) {

						hadRewarded = HasRewarded;
						RewardedAvailabilityHasChanged?.Invoke();
					}
				}

				MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (unitId, adInfo) => CheckInterstitialAvailabilityChange();
				MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (unitId, errorInfo) => CheckInterstitialAvailabilityChange();
				MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (unitId, adInfo) => CheckInterstitialAvailabilityChange();
				MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (unitId, errorInfo, adInfo) => CheckInterstitialAvailabilityChange();
				MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (unitId, adInfo) => RevenuePaid(AnalyticsHelper.AdvertType.Interstitial, unitId, adInfo);

				void CheckInterstitialAvailabilityChange() {

					if (HasInterstitial != hadInterstitial) {

						hadInterstitial = HasInterstitial;
						InterstitialAvailabilityHasChanged?.Invoke();
					}
				}

				MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (unitId, adInfo) => RevenuePaid(AnalyticsHelper.AdvertType.Banner, unitId, adInfo);

				void RevenuePaid(AnalyticsHelper.AdvertType adType, string unit, MaxSdkBase.AdInfo adInfo) {

					AnalyticsHelper.RevenueData revenueData = new AnalyticsHelper.RevenueData(adType, "AppLovin", "USD", adInfo.Revenue);

					revenueData.AdUnitId = adInfo.AdUnitIdentifier;
					revenueData.NetworkName = adInfo.NetworkName;
					revenueData.PlacementName = adInfo.Placement;

					AdvertRevenueHasPaid?.Invoke(revenueData);
				}
			}

			public bool RequestRewarded(Action onComplete, Action<string> onFailed) {

				if (SupportsRewarded) {

					RegisterEvents();
					MaxSdk.LoadRewardedAd(rewardedAdUnit);

					return true;

					void RegisterEvents() {

						MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdLoadedEvent;
						MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
					}

					void UnregisterEvents() {

						MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnAdLoadedEvent;
						MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnAdLoadFailedEvent;
					}

					void OnAdLoadedEvent(string adUnit, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onComplete?.Invoke();
					}

					void OnAdLoadFailedEvent(string adUnit, MaxSdk.ErrorInfo errorInfo) {

						UnregisterEvents();
						onFailed?.Invoke(errorInfo.ToString());
					}
				}

				return false;
			}

			public bool ShowRewarded(Action onComplete, Action<string> onFailed, Action<string, double> onReward) {

				if (HasRewarded) {

					RegisterEvents();
					MaxSdk.ShowRewardedAd(rewardedAdUnit);

					return true;

					void RegisterEvents() {

						MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHiddenEvent;
						MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
						MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
					}

					void UnregisterEvents() {

						MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnAdHiddenEvent;
						MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnAdDisplayFailedEvent;
						MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardEvent;
					}

					void OnAdHiddenEvent(string adUnit, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onComplete?.Invoke();
					}

					void OnAdDisplayFailedEvent(string adUnit, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onFailed?.Invoke(errorInfo.ToString());
					}

					void OnAdReceivedRewardEvent(string adUnit, MaxSdk.Reward reward, MaxSdk.AdInfo adInfo) {

						onReward?.Invoke(reward.Label, reward.Amount);
					}
				}

				return false;
			}

			public bool RequestInterstitial(Action onComplete, Action<string> onFailed) {

				if (SupportsInterstitial) {

					RegisterEvents();
					MaxSdk.LoadInterstitial(interstitialAdUnit);

					return true;

					void RegisterEvents() {

						MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoadedEvent;
						MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
					}

					void UnregisterEvents() {

						MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnAdLoadedEvent;
						MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnAdLoadFailedEvent;
					}

					void OnAdLoadedEvent(string adUnit, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onComplete?.Invoke();
					}

					void OnAdLoadFailedEvent(string adUnit, MaxSdk.ErrorInfo errorInfo) {

						UnregisterEvents();
						onFailed?.Invoke(errorInfo.ToString());
					}
				}

				return false;
			}

			public bool ShowInterstitial(Action onComplete, Action<string> onFailed) {

				if (HasInterstitial) {

					RegisterEvents();
					MaxSdk.ShowInterstitial(interstitialAdUnit);

					return true;

					void RegisterEvents() {

						MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHiddenEvent;
						MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
					}

					void UnregisterEvents() {

						MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnAdHiddenEvent;
						MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnAdDisplayFailedEvent;
					}

					void OnAdHiddenEvent(string adUnit, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onComplete?.Invoke();
					}

					void OnAdDisplayFailedEvent(string adUnit, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						onFailed?.Invoke(errorInfo.ToString());
					}
				}

				return false;
			}

			private MaxSdk.BannerPosition GetBannerPosition(BannerPosition position) {

				switch (position) {
					case BannerPosition.Top:
						return MaxSdkBase.BannerPosition.TopCenter;
					case BannerPosition.Bottom:
						return MaxSdkBase.BannerPosition.BottomCenter;
					default:
						throw new NotImplementedException("Banner position: " + position + ": has not been implemented");
				}
			}

			public bool RequestBanner(BannerPosition position, Action onComplete, Action<string> onFailed) {

				if (SupportsBanners && !loadingBanner && !bannerLoaded) {

					loadingBanner = true;
					RegisterEvents();
					MaxSdk.CreateBanner(bannerAdUnit, GetBannerPosition(position));

					return true;

					void RegisterEvents() {

						MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
						MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
					}

					void UnregisterEvents() {

						MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnAdLoadedEvent;
						MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnAdLoadFailedEvent;
					}

					void OnAdLoadedEvent(string adUnit, MaxSdk.AdInfo adInfo) {

						UnregisterEvents();
						loadingBanner = false;
						bannerLoaded = true;
						onComplete?.Invoke();
					}

					void OnAdLoadFailedEvent(string adUnit, MaxSdk.ErrorInfo errorInfo) {

						UnregisterEvents();
						loadingBanner = false;
						onFailed?.Invoke(errorInfo.ToString());
					}
				}

				return false;
			}

			public bool ShowBanner() {

				if (HasBanner) {

					MaxSdk.ShowBanner(bannerAdUnit);
					bannerShown = true;

					return true;
				}

				return false;
			}

			public bool HideBanner() {

				if (HasBanner) {

					MaxSdk.HideBanner(bannerAdUnit);
					bannerShown = false;

					return true;
				}

				return false;
			}

			public bool SetBannerPosition(BannerPosition position) {

				if (HasBanner) {

					MaxSdk.UpdateBannerPosition(bannerAdUnit, GetBannerPosition(position));
					return true;
				}

				return false;
			}

			public void ShowDebugger() {

				MaxSdk.ShowMediationDebugger();
			}
		}
	}
}
