using NeonPlay.Helper;
using NeonPlayHelper;
using NeonPlayUi;
using System;
using UnityEngine;

namespace NeonPlay {

	public class PlayPack : MonoBehaviour {

		private const string KeyPrefx = "PlayPack";
		private const string SurveySeenDayKey = KeyPrefx + "_SurveySeenDay";
		private const string SurveySeenCountKey = KeyPrefx + "_SurveySeenCount";
		private const int MaximumSurveySeenCount = 3;

		private static float advertRequestTimer;
		private static float interstitalAllowTime;
		private static NeonPlayConfiguration.PlayPack playPackConfiguration;
		private static PrivacyScreen privacyScreenInstance;
		private static RatePopup ratePopupInstance;
		private static Action<double> onlineAction;

		private static bool HasConfiguration => playPackConfiguration != null;
		private static float AdvertRequestInterval => playPackConfiguration.AdvertRequestInterval;
		private static AdvertHelper.BannerPosition BannerPosition => playPackConfiguration.BannerPosition;
		private static PrivacyScreen PrivacyScreenPrefab => playPackConfiguration.PrivacyScreen;
		private static RatePopup RatePopupPrefab => playPackConfiguration.RatePopup;
		private static float InterstitialBlockTime => playPackConfiguration.InterstitialBlockTime;
		private static float InitialInterstitialBlockTime => playPackConfiguration.InitialInterstitialBlockTime;
		private static string SupportEmailAddress => HasConfiguration ? playPackConfiguration.SupportEmailAddress : null;

		public static bool HasPrivacyScreen => privacyScreenInstance != null;
		public static bool HasRatePopup => ratePopupInstance != null;
		public static bool HasSurvey => !string.IsNullOrEmpty(playPackConfiguration.SurveyUrl) && !string.IsNullOrEmpty(playPackConfiguration.SurveyKey);
		public static bool CanShowRewardedVideo => AdvertHelper.HasRewarded;
		public static bool CanShowInterstitial => AdvertHelper.HasInterstitial && InterstitialAllowed;
		public static bool CanShowPrivacySettings => HasPrivacyScreen && privacyScreenInstance.CanShowSettings;
		public static bool CanShowSurvey => HasSurvey && !SurveyHelper.SurveyRewardGiven && !SurveySeenToday && (SurveySeenCount < MaximumSurveySeenCount);
		public static bool SurveySeenToday => SurveySeenDay >= DayHelper.CurrentDay;
		public static bool InterstitialAllowed => HasConfiguration && (Time.realtimeSinceStartup >= interstitalAllowTime);
		public static bool BannerVisible => AdvertHelper.BannerVisible;
		public static bool HidingBanner { get; private set; }

		public static int SurveySeenDay {
			get => SaveGameHelper.GetInt(SurveySeenDayKey, 0);
			set => SaveGameHelper.SetInt(SurveySeenDayKey, value);
		}

		public static int SurveySeenCount {
			get => SaveGameHelper.GetInt(SurveySeenCountKey, 0);
			set => SaveGameHelper.SetInt(SurveySeenCountKey, value);
		}

		protected virtual void Start() {

			enabled = false;
			playPackConfiguration = NeonPlayConfiguration.RuntimeInstance.PlayPackConfiguration;

			AbHelper.Initialise();

			OfflineHelper.MinimumOfflineDuration = playPackConfiguration.MinimumOfflineDuration;
			OfflineHelper.MaximumOfflineDuration = playPackConfiguration.MaximumOfflineDuration;

			if (PrivacyScreenPrefab != null) {

				privacyScreenInstance = Instantiate(PrivacyScreenPrefab);
				DontDestroyOnLoad(privacyScreenInstance.gameObject);
			}

			if (RatePopupPrefab != null) {

				ratePopupInstance = Instantiate(RatePopupPrefab);
				DontDestroyOnLoad(ratePopupInstance);
			}

			AdvertHelper.AdvertRevenueHasPaid += AnalyticsHelper.LogAdvertRevenue;

			ResetAdvertTimers(InitialInterstitialBlockTime);

			if (!AdvertisingIdentifierHelper.RequestPermission(OnPermissionRequested)) {

				OnPermissionRequested();
			}

			if (HasSurvey) {

				SurveyHelper.Setup(IdHelper.PlayerId, IdHelper.DeepLinkId, playPackConfiguration.SurveyKey);
			}

			DontDestroyOnLoad(gameObject);
		}

		protected virtual void OnDestroy() {

			AdvertHelper.AdvertRevenueHasPaid -= AnalyticsHelper.LogAdvertRevenue;
			OfflineHelper.OfflineHasEnded -= OnOnline;
		}

		private void OnPermissionRequested() {

			PrivacyHelper.Initialise();
			AnalyticsHelper.Initialise();
			AnalyticsHelper.SetAbMode(AbHelper.Mode);

			if (!Activate(ShowWelcome)) {

				ShowWelcome();
			}
		}

		private void ShowWelcome(Action onComplete = null) {

			if (!HasPrivacyScreen || !privacyScreenInstance.ShowWelcome(Done)) {

				Done();
			}

			void Done() {

				UpdateConsent();

				onComplete?.Invoke();
			}
		}

		public static void RegisterOnline(Action<double> onOnline) {

			if (onlineAction != null) {

				throw new InvalidOperationException("The online action can only be registered once");
			}

			onlineAction = onOnline ?? throw new ArgumentNullException("onOnline");

			OfflineHelper.OfflineHasEnded += OnOnline;
			OfflineHelper.Activate();
		}

		public static void UnregisterOnline() {

			onlineAction = null;
			OfflineHelper.OfflineHasEnded -= OnOnline;
		}

		protected static void OnOnline(double duration) {

			onlineAction?.Invoke(duration);
		}

		private void UpdateConsent() {

			AnalyticsHelper.UpdateConsent();
			AdvertHelper.UpdateConsent();
		}

		private bool Activate(Action<Action> onComplete) {

			return AdvertHelper.Initialise(onComplete: () => {

				onComplete?.Invoke(() => {

					enabled = true;
				});
			});
		}

		protected virtual void Update() {

			if (AdvertRequestInterval > 0.0f) {

				advertRequestTimer -= Time.unscaledDeltaTime;

				if (advertRequestTimer <= 0.0f) {

					advertRequestTimer += AdvertRequestInterval;

					AdvertHelper.RequestBanner(BannerPosition, () => {

						if (HidingBanner) {

							AdvertHelper.HideBanner();

						} else {

							AdvertHelper.ShowBanner();
						}

					}, null);

					AdvertHelper.RequestRewarded(null, null);
					AdvertHelper.RequestInterstitial(null, null);
				}
			}
		}

		private static void ResetAdvertTimers(float interstitialBlockTime) {

			advertRequestTimer = 0.0f;
			interstitalAllowTime = Time.realtimeSinceStartup + interstitialBlockTime;
		}

		public static bool ShowRewardedVideo(Action onDone, Action<string, double> onReward = null) {

			return AdvertHelper.ShowRewarded(() => {

				ResetAdvertTimers(InterstitialBlockTime);
				onDone?.Invoke();

			}, error => onDone?.Invoke(), onReward);
		}

		public static bool ShowInterstitialAdvert(Action onDone) {

			return InterstitialAllowed && AdvertHelper.ShowInterstitial(() => {

				ResetAdvertTimers(InterstitialBlockTime);
				onDone?.Invoke();

			}, error => onDone?.Invoke());
		}

		public static void ShowAdvertDebugger() {

			AdvertHelper.ShowDebugger();
		}

		public static bool ShowRateDialog(Action onComplete = null) {

			return HasRatePopup && ratePopupInstance.Show(onComplete);
		}

		public static void HideBanner() {

			HidingBanner = true;
			AdvertHelper.HideBanner();
		}

		public static void ShowBanner() {

			HidingBanner = false;
			AdvertHelper.ShowBanner();
		}

		public static bool ShowPrivacySettings(Action onComplete = null) {

			return HasPrivacyScreen && privacyScreenInstance.ShowSettings(onComplete);
		}

		public static void SendSupportEmail(string body = null) {

			if (!string.IsNullOrEmpty(SupportEmailAddress)) {

				string subject = "Help with " + Application.productName;

				EmailHelper.SendEmail(SupportEmailAddress, subject, body);
			}
		}

		public static bool OpenSurvey(Action onReward) {

			if (HasSurvey) {

				if (SurveyHelper.Open(IdHelper.PlayerId, IdHelper.DeepLinkId, playPackConfiguration.SurveyUrl, () => SurveyHelper.SurveyHasCompleted -= onReward)) {

					SurveyHelper.SurveyHasCompleted += onReward;

					return true;
				}
			}

			return false;
		}

		public static void LogDesignEvent(params string[] parameterList) {

			AnalyticsHelper.LogDesignEvent(parameterList);
		}

		public static void LogDesignEvent(float value, params string[] parameterList) {

			AnalyticsHelper.LogDesignEvent(value, parameterList);
		}
	}
}
