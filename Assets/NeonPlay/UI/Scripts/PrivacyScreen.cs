using NeonPlay.Helper;
using NeonPlayHelper;
using System;
using TMPro;
using UnityEngine;

namespace NeonPlayUi {

	public class PrivacyScreen : MonoBehaviour {

		[SerializeField]
		private PrivacySelector privacySelector;

		[SerializeField]
		private GameObject background;

		[SerializeField]
		private GameObject foreground;

		[SerializeField]
		private PrivacySwitchSlider analyticsSwitchSlider;

		[SerializeField]
		private PrivacySwitchSlider trackingSwitchSlider;

		[SerializeField]
		private PrivacySwitchSlider ccpaShareSwitchSlider;

		[SerializeField]
		private TMP_InputField nameInputField;

		[SerializeField]
		private TMP_InputField emailInputField;

		[SerializeField]
		private GameObject[] iosGameObjectList;

		[SerializeField]
		private GameObject[] androidGameObjectList;

		[SerializeField]
		private GameObject[] iosTrackingGameObjectList;

		private string advertisingIdentifier;
		private bool trackingEnabled;
#if UNITY_IOS
		private bool IosTrackingDisabled => !trackingEnabled;
#else
		private bool IosTrackingDisabled => false;
#endif

		public bool TrackingAccepted { get; set; }
		public bool AnalyticsAccepted { get; set; }
		public bool DoNotSell { get; set; }

		public bool CanShowSettings => PrivacyHelper.GdprApplicable || PrivacyHelper.CcpaApplicable;

		public string Name { get; set; }
		public string Email { get; set; }

		private Action completionAction;
		private bool delete;

		protected virtual void Start() {

#if UNITY_IOS
			bool isIos = true;
#else
			bool isIos = false;
#endif

			background.SetActive(false);
			foreground.SetActive(false);

			foreach (GameObject gameObject in iosGameObjectList) {

				gameObject.SetActive(isIos);
			}

			foreach (GameObject gameObject in androidGameObjectList) {

				gameObject.SetActive(!isIos);
			}
		}

		public string GetPrivacyText(PrivacyText.Entry entry) {

			switch (entry) {
				case PrivacyText.Entry.WelcomeTitle:
					return "Make " + Application.productName + " better";
				case PrivacyText.Entry.WelcomeBody:
					if (IosTrackingDisabled) {
						return "Hi, before you get started, we'd like to ask you about the data we can collect while you are playing.<br><br>We will use it to make the game even more fun, make technical improvements and fix bugs! This data is not used for tracking.<br><br>If you are 16 or over and are happy to let us use your data, please click the button below. You can adjust your privacy settings at any time.<br><br>Thank you for supporting us.";
					}
					return "Hi, before you get started, we'd like to ask you about the data we can collect while you are playing.<br><br>We will use it to make the game even more fun, make technical improvements and fix bugs! We will also be able to show ads that are relevant to you.<br><br>If you are 16 or over and are happy to let us use your data, please click the button below. You can adjust your privacy settings at any time.<br><br>Thank you for supporting us.";
				case PrivacyText.Entry.AcceptButton:
					return "<size=100%>Awesome!<br><size=50%>I support that";
				case PrivacyText.Entry.RejectButton:
					return "Manage data settings";
				case PrivacyText.Entry.ConfirmTitle:
					return "How your data makes<br>" + Application.productName + " better";
				case PrivacyText.Entry.ConfirmBody:
					return "We'd like to let you know what sending us your data helps us to do:";
				case PrivacyText.Entry.ConfirmAnalytics:
					return "Sending us data about the way you play the game helps us improve it and make it more enjoyable for everyone";
				case PrivacyText.Entry.ConfirmBugTracking:
					return "Sending information about bugs and crashes helps us improve future versions of the game";
				case PrivacyText.Entry.ConfirmTargetted:
					return "Allows us to show ads that are relevant to you";
				case PrivacyText.Entry.ConfirmAge:
					return "If you are 16 or over, press the button to start playing";
				case PrivacyText.Entry.SettingsTitle:
					return "Manage Data Use";
				case PrivacyText.Entry.SettingsBody:
					if (IosTrackingDisabled) {
						return "Sharing your data will enable us to support and improve the game. This data is not used for tracking.";
					}
					return "Sharing your data will enable us to show more relevant ads and support and improve the game.  We will still show ads if you turn off targeted ads and you will find they are less relevant.";
				case PrivacyText.Entry.SettingsAnalytics:
					return "I agree to the collection of data to help improve the game";
				case PrivacyText.Entry.SettingsAds:
					return "I agree to receive personalised ads";
				case PrivacyText.Entry.SettingsInformation:
					return "More Information";
				case PrivacyText.Entry.SettingsClose:
					return "Let's play";
				case PrivacyText.Entry.InformationTitle:
					return "Privacy and your data";
				case PrivacyText.Entry.InformationPrivacy:
					return "Privacy policy:<br><color=#7f7fff>https://neonplay.com/privacy-policy</color>";
				case PrivacyText.Entry.InformationPartner:
					return "Partner privacy policies:<br><color=#7f7fff>https://neonplay.com/inapp-privacy</color>";
				case PrivacyText.Entry.InformationForget:
					return "Forget me";
				case PrivacyText.Entry.InformationAccess:
					return "Access my data";
				case PrivacyText.Entry.InformationBack:
					return "Back";
				case PrivacyText.Entry.AccessTitle:
					if (delete) {
						return "Delete my data";
					}
					return "Access my data";
				case PrivacyText.Entry.AccessBody:
					return "You are entitled to access and/or delete your data";
				case PrivacyText.Entry.AccessIdfa:
					return advertisingIdentifier;
				case PrivacyText.Entry.AccessResetIos:
					return "You can also reset your IDFA:<br><color=#7f7fff>https://support.apple.com/en-gb/HT205223</color>";
				case PrivacyText.Entry.AccessResetAndroid:
					return "You can also reset your GAID: <color=#7f7fff>https://support.google.com/googleplay/answer/3405269</color>";
				case PrivacyText.Entry.AccessSend:
					return "Send request";
				case PrivacyText.Entry.AccessBack:
					return "Back";
				case PrivacyText.Entry.CcpaTitle:
					return "Manage my data";
				case PrivacyText.Entry.CcpaBody:
					return "We may share your cookie information and advertising identifiers with our third-party advertising platforms to deliver interest-based advertisements, attribute installs and optimize campaign performance.";
				case PrivacyText.Entry.CcpaShare:
					return "Share data";
				default:
					return string.Empty;
			}
		}

		private void Show(PrivacySelector.PrivacyPage privacyPage, Action onComplete) {

			completionAction = onComplete;
			background.SetActive(true);

			if (!AdvertisingIdentifierHelper.RequestIdentifier(ShowForeground)) {

				ShowForeground(string.Empty, false, string.Empty);
			}

			void ShowForeground(string id, bool enabled, string error) {

				advertisingIdentifier = id;
				trackingEnabled = enabled;
				foreground.SetActive(true);
				privacySelector.SelectPage(privacyPage);

				foreach (GameObject gameObject in iosTrackingGameObjectList) {

					gameObject.SetActive(!IosTrackingDisabled);
				}
			}
		}

		private void Complete() {

			background.SetActive(false);
			foreground.SetActive(false);
			completionAction?.Invoke();
			completionAction = null;
		}

		public bool ShowWelcome(Action onComplete) {

			if (PrivacyHelper.GdprApplicable && !PrivacyHelper.GdprComplete) {

				Show(PrivacySelector.PrivacyPage.Welcome, onComplete);

				return true;
			}

			return false;
		}

		public bool ShowSettings(Action onComplete) {

			TrackingAccepted = PrivacyHelper.GdprAcceptedTracking;
			AnalyticsAccepted = PrivacyHelper.GdprAcceptedAnalytics;
			DoNotSell = PrivacyHelper.CcpaDoNotSell;

			analyticsSwitchSlider.Value = AnalyticsAccepted;
			trackingSwitchSlider.Value = TrackingAccepted;
			ccpaShareSwitchSlider.Value = !DoNotSell;

			if (PrivacyHelper.GdprApplicable) {

				Show(PrivacySelector.PrivacyPage.Settings, onComplete);

				return true;

			} else if (PrivacyHelper.CcpaApplicable) {

				Show(PrivacySelector.PrivacyPage.Ccpa, onComplete);

				return true;
			}

			return false;
		}

		public void OnAccept() {

			PrivacyHelper.GdprAgeConfirmed = true;
			PrivacyHelper.GdprAcceptedTracking = true;
			PrivacyHelper.GdprAcceptedAnalytics = true;
			PrivacyHelper.GdprComplete = true;

			Complete();
		}

		public void OnReject() {

			switch (privacySelector.CurrentPage) {
				case PrivacySelector.PrivacyPage.Welcome:
					privacySelector.SelectPage(PrivacySelector.PrivacyPage.Confirm);
					break;
				case PrivacySelector.PrivacyPage.Confirm:
					analyticsSwitchSlider.Value = AnalyticsAccepted = true;
					trackingSwitchSlider.Value = TrackingAccepted = true;
					privacySelector.SelectPage(PrivacySelector.PrivacyPage.Settings);
					break;
			}
		}

		public void OnApplySettings(string privacyPageString) {

			if (Enum.TryParse(privacyPageString, out PrivacySelector.PrivacyPage privacyPage)) {

				switch (privacyPage) {
					case PrivacySelector.PrivacyPage.Settings:
						PrivacyHelper.GdprAcceptedTracking = TrackingAccepted;
						PrivacyHelper.GdprAcceptedAnalytics = AnalyticsAccepted;
						break;
					case PrivacySelector.PrivacyPage.Ccpa:
						PrivacyHelper.CcpaDoNotSell = DoNotSell;
						break;
				}

			} else {

				Debug.LogError("Unknown privacy page: " + privacyPageString);
			}

			PrivacyHelper.GdprComplete = true;
			Complete();
		}

		public void OnAnalyticsSwitch() {

			AnalyticsAccepted = !AnalyticsAccepted;
			analyticsSwitchSlider.Value = AnalyticsAccepted;
		}

		public void OnTrackingSwitch() {

			TrackingAccepted = !TrackingAccepted;
			trackingSwitchSlider.Value = TrackingAccepted;
		}

		public void OnCcpaShareSwitch() {

			DoNotSell = !DoNotSell;
			ccpaShareSwitchSlider.Value = !DoNotSell;
		}

		public void OnInformationButton() {

			privacySelector.SelectPage(PrivacySelector.PrivacyPage.Information);
		}

		public void OnForgetButton() {

			if (trackingEnabled) {

				delete = true;
				privacySelector.SelectPage(PrivacySelector.PrivacyPage.Access);
			}
		}

		public void OnAccessButton() {

			if (trackingEnabled) {

				delete = false;
				privacySelector.SelectPage(PrivacySelector.PrivacyPage.Access);
			}
		}

		public void OnBackButton() {

			switch (privacySelector.CurrentPage) {
				case PrivacySelector.PrivacyPage.Access:
					privacySelector.SelectPage(PrivacySelector.PrivacyPage.Information);
					break;
				case PrivacySelector.PrivacyPage.Information:
					privacySelector.SelectPage(PrivacySelector.PrivacyPage.Settings);
					break;
			}
		}

		public void OnEmailEdit() {

			Email = emailInputField.text;
		}

		public void OnNameEdit() {

			Name = nameInputField.text;
		}

		public void OnSendRequestButton() {

			if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email)) {

				string address = "gdpr@gdpr.neonplay.com";
				string identifier = advertisingIdentifier;
				string mode = delete ? "Delete" : "Access";
#if UNITY_IOS
				string platform = "iOS";
#elif UNITY_ANDROID
				string platform = "Android";
#endif
				string subject = mode + " my data [" + identifier + "]";
				string body = "mode: " + mode + "\nname: " + Name + "\nemail: " + Email + "\nadvertising_identifier: [" + identifier + "]\nplatform: " + platform + "\ngame: " + Application.productName;

				EmailHelper.SendEmail(address, subject, body);
			}
		}

		public void OnOpenUrl(string url) {

			Application.OpenURL(url);
		}
	}
}
