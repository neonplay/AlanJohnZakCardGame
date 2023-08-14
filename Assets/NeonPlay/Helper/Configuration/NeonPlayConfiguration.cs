using NeonPlayUi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonPlayHelper {

	public class NeonPlayConfiguration : ScriptableObject {

		[Serializable] public class GameAnalytics {

			[SerializeField] private string iosGameKey;
			[SerializeField] private string iosSecretKey;
			[SerializeField] private string androidGameKey;
			[SerializeField] private string androidSecretKey;

			public string IosGameKey => iosGameKey;
			public string IosSecretKey => iosSecretKey;
			public string AndroidGameKey => androidGameKey;
			public string AndroidSecretKey => androidSecretKey;
		}

		[Serializable] public class Advert {

			[Serializable] public class IdSet {

				[SerializeField] private string bannerId;
				[SerializeField] private string interstitialId;
				[SerializeField] private string rewardedId;
				[SerializeField] private string adMobId;

				public string BannerId => bannerId;
				public string InterstitialId => interstitialId;
				public string RewardedId => rewardedId;
				public string AdMobId => adMobId;
			}

			[SerializeField] private IdSet iosIdSet;
			[SerializeField] private IdSet androidIdSet;

			public IdSet IosIdSet => iosIdSet;
			public IdSet AndroidIdSet => androidIdSet;

#if UNITY_IOS
			public IdSet CurrentIdSet => iosIdSet;
#elif UNITY_ANDROID
			public IdSet CurrentIdSet => androidIdSet;
#endif
		}

		[Serializable] public class Ab {

			public enum Phase {
				Alpha,
				Bravo,
				Charlie,
				Delta,
				Echo,
				Foxtrot,
				Golf,
				Hotel,
				India,
				Juliett,
				Kilo,
				Lima,
				Mike,
				November,
				Oscar,
				Papa,
				Quebec,
				Romeo,
				Sierra,
				Tango,
				Uniform,
				Victor,
				Whiskey,
				XRay,
				Yankee,
				Zulu
			}

			[SerializeField] private Phase abPhase;
			[SerializeField] private int individualPercentage = 5;
			[SerializeField] private string[] modeList;

			public Phase AbPhase => abPhase;
			public int IndividualPercentage => individualPercentage;
			public IEnumerable<string> ModeList => modeList;
		}

		[Serializable] public class PlayPack {

			[SerializeField] private float advertRequestInterval = 10.0f;
			[SerializeField] private float interstitialBlockTime = 30.0f;
			[SerializeField] private float initialInterstitialBlockTime = 60.0f;
			[SerializeField] private double minimumOfflineDuration = 10.0 * 60.0;
			[SerializeField] private double maximumOfflineDuration = 8.0 * 60.0 * 60.0;
			[SerializeField] private AdvertHelper.BannerPosition bannerPosition = AdvertHelper.BannerPosition.Bottom;
			[SerializeField] private PrivacyScreen privacyScreen;
			[SerializeField] private RatePopup ratePopup;
			[SerializeField] private string supportEmailAddress;
			[SerializeField] private string surveyUrl;
			[SerializeField] private string surveyKey;

			public float AdvertRequestInterval => advertRequestInterval;
			public AdvertHelper.BannerPosition BannerPosition => bannerPosition;
			public PrivacyScreen PrivacyScreen => privacyScreen;
			public float InterstitialBlockTime => interstitialBlockTime;
			public float InitialInterstitialBlockTime => initialInterstitialBlockTime;
			public double MinimumOfflineDuration => minimumOfflineDuration;
			public double MaximumOfflineDuration => maximumOfflineDuration;
			public RatePopup RatePopup => ratePopup;
			public string SupportEmailAddress => supportEmailAddress;
			public string SurveyUrl => surveyUrl;
			public string SurveyKey => surveyKey;
		}

		[SerializeField] private GameAnalytics gameAnalyticsConfiguration;
		[SerializeField] private Advert advertConfiguration;
		[SerializeField] private Ab abConfiguration;
		[SerializeField] private PlayPack playPackConfiguration;

		public GameAnalytics GameAnalyticsConfiguration => gameAnalyticsConfiguration;
		public Advert AdvertConfiguration => advertConfiguration;
		public Ab AbConfiguration => abConfiguration;
		public PlayPack PlayPackConfiguration => playPackConfiguration;

		public static NeonPlayConfiguration RuntimeInstance => Resources.Load<NeonPlayConfiguration>("Configuration/NeonPlayConfiguration");
    }
}
