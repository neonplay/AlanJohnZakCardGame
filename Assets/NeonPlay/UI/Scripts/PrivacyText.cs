using TMPro;
using UnityEngine;

namespace NeonPlayUi {

	public class PrivacyText : NeonPlayUiText {

		public enum Entry {
			WelcomeTitle,
			WelcomeBody,
			AcceptButton,
			RejectButton,
			ConfirmTitle,
			ConfirmBody,
			ConfirmAnalytics,
			ConfirmBugTracking,
			ConfirmTargetted,
			ConfirmAge,
			SettingsTitle,
			SettingsBody,
			SettingsAnalytics,
			SettingsAds,
			SettingsInformation,
			SettingsClose,
			InformationTitle,
			InformationPrivacy,
			InformationPartner,
			InformationForget,
			InformationAccess,
			InformationBack,
			AccessTitle,
			AccessBody,
			AccessIdfa,
			AccessResetIos,
			AccessResetAndroid,
			AccessSend,
			AccessBack,
			CcpaTitle,
			CcpaBody,
			CcpaShare
		}

		[SerializeField]
		private Entry entry;

		[SerializeField]
		private PrivacyScreen privacyScreen;

		protected override void Start() {

			Text.text = privacyScreen.GetPrivacyText(entry);
		}

#if UNITY_EDITOR
		protected override void OnValidate() {

			base.OnValidate();

			if (privacyScreen == null) {

				privacyScreen = GetComponentInParent<PrivacyScreen>();
			}
		}
#endif
	}
}
