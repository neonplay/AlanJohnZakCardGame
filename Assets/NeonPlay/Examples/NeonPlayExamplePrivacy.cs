using NeonPlayHelper;
using NeonPlayUi;
using System;
using TMPro;
using UnityEngine;

namespace NeonPlayExample {

	public class NeonPlayExamplePrivacy : MonoBehaviour {

		[SerializeField]
		private TextMeshProUGUI advertisingIdentiferText;

		[SerializeField]
		private NeonPlayExampleAdverts exampleAdverts;

		[SerializeField]
		private PrivacyScreen privacyScreen;

		protected virtual void Start() {

			if (!AdvertisingIdentifierHelper.RequestPermission(OnPermissionRequested)) {

				OnPermissionRequested();
			}
		}

		private void OnPermissionRequested() {

			PrivacyHelper.Initialise();
			AnalyticsHelper.Initialise();

			if (!exampleAdverts.Activate(ShowWelcome)) {

				ShowWelcome();
			}

			advertisingIdentiferText.text = "Advertising Identifier: ";

			if (!AdvertisingIdentifierHelper.RequestIdentifier((id, trackingEnabled, error) => {

				if (trackingEnabled) {

					advertisingIdentiferText.text += id;

				} else {

					advertisingIdentiferText.text += error;
				}

			})) {

				advertisingIdentiferText.text += "Unavailable";
			}
		}

		private void ShowWelcome(Action onComplete = null) {

			if (!privacyScreen.ShowWelcome(Done)) {

				Done();
			}

			void Done() {

				UpdateConsent();

				onComplete?.Invoke();
			}
		}

		private void UpdateConsent() {

			AnalyticsHelper.UpdateConsent();
			AdvertHelper.UpdateConsent();
		}

		public void OnPrivacyButton() {

			privacyScreen.ShowSettings(UpdateConsent);
		}
	}
}
