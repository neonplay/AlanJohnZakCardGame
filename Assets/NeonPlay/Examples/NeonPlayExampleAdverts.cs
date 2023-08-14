using NeonPlayHelper;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonPlayExample {

	public class NeonPlayExampleAdverts : MonoBehaviour {

		[SerializeField]
		private Button rewardedAdvertButton;

		[SerializeField]
		private TextMeshProUGUI rewardedAdvertStatusText;

		[SerializeField]
		private TextMeshProUGUI rewardedAdvertResultText;

		[SerializeField]
		private TextMeshProUGUI interstitialAdvertStatusText;

		[SerializeField]
		private TextMeshProUGUI interstitialAdvertResultText;

		[SerializeField]
		private TextMeshProUGUI interstitialAdvertTimerText;

		[SerializeField]
		private float requestInterval = 10.0f;

		[SerializeField]
		private float interstitialInterval = 60.0f;

		private float requestTimer;
		private float interstitialTimer;

		protected virtual void Start() {

			enabled = false;

			OnRewardedAdvertAvailabilityChanged();
			OnInterstitialAdvertAvailabilityChanged();

			AdvertHelper.RewardedAvailabilityHasChanged += OnRewardedAdvertAvailabilityChanged;
			AdvertHelper.InterstitialAvailabilityHasChanged += OnInterstitialAdvertAvailabilityChanged;
			AdvertHelper.AdvertRevenueHasPaid += AnalyticsHelper.LogAdvertRevenue;

			ResetRequestTimer();
			ResetInterstitialTimer();
		}

		public bool Activate(Action<Action> onComplete) {

			return AdvertHelper.Initialise(onComplete: () => {

				onComplete?.Invoke(() => {

					enabled = true;
				});
			});
		}

		protected virtual void Update() {

			if (requestInterval > 0.0f) {

				requestTimer -= Time.unscaledDeltaTime;

				if (requestTimer <= 0.0f) {

					requestTimer += requestInterval;

					AdvertHelper.RequestBanner(AdvertHelper.BannerPosition.Bottom, () => AdvertHelper.ShowBanner(), null);
					AdvertHelper.RequestRewarded(null, null);
					AdvertHelper.RequestInterstitial(null, null);
				}
			}

			if (interstitialInterval > 0.0f) {

				interstitialTimer -= Time.unscaledDeltaTime;

				if (interstitialTimer <= 0.0f) {

					interstitialTimer += interstitialInterval;

					ShowInterstitialAdvert();
				}

				interstitialAdvertTimerText.text = interstitialTimer.ToString("00") + "s";

			} else {

				interstitialAdvertTimerText.text = string.Empty;
			}
		}

		private void ResetRequestTimer() {

			requestTimer = 0.0f;
		}

		private void ResetInterstitialTimer() {

			interstitialTimer = interstitialInterval;
		}

		protected virtual void OnDestroy() {

			AdvertHelper.RewardedAvailabilityHasChanged -= OnRewardedAdvertAvailabilityChanged;
			AdvertHelper.InterstitialAvailabilityHasChanged -= OnInterstitialAdvertAvailabilityChanged;
			AdvertHelper.AdvertRevenueHasPaid -= AnalyticsHelper.LogAdvertRevenue;
		}

		private void OnRewardedAdvertAvailabilityChanged() {

			rewardedAdvertStatusText.text = AdvertHelper.HasRewarded ? "Rewarded Advert Available" : "No Rewarded Advert Available";
			rewardedAdvertButton.interactable = AdvertHelper.HasRewarded;
		}

		private void OnInterstitialAdvertAvailabilityChanged() {

			interstitialAdvertStatusText.text = AdvertHelper.HasInterstitial ? "Interstitial Advert Available" : "No Interstitial Advert Available";
		}

		public void OnShowRewardedAdvert() {

			if (AdvertHelper.ShowRewarded(() => {

				ResetRequestTimer();
				ResetInterstitialTimer();

				rewardedAdvertResultText.text = "Rewarded Advert Complete";

			}, error => rewardedAdvertResultText.text = "Rewarded Advert Error: " + error)) {

				rewardedAdvertResultText.text = "Showing Rewarded Advert";

			} else {

				rewardedAdvertResultText.text = "Showing Rewarded Advert Failed";
			}
		}

		public void ShowInterstitialAdvert() {

			if (AdvertHelper.ShowInterstitial(() => {

				ResetRequestTimer();
				ResetInterstitialTimer();

				interstitialAdvertResultText.text = "Intestitial Advert Complete";

			}, error => interstitialAdvertResultText.text = "Interstitial Advert Error: " + error)) {

				interstitialAdvertResultText.text = "Showing Interstitial Advert";

			} else {

				interstitialAdvertResultText.text = "Showing Interstitial Advert Failed";
			}
		}

		public void OnDebugAdverts() {

			AdvertHelper.ShowDebugger();
		}
	}
}