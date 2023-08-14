using NeonPlay;
using NeonPlay.Helper;
using NeonPlayHelper;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonPlayExample {

	public class PlayPackExample : MonoBehaviour {

		public GameObject privacyButtonGameObject;
		public GameObject surveyButtonGameObject;
		public GameObject rvButtonGameObject;
		public GameObject fakeMenuGameObject;
		public GameObject rewardGameObject;

		public Button menuButton;
		public Button rvButton;

		public TextMeshProUGUI progressText;

		private bool CanOfferReward => !RewardActive && PlayPack.CanShowRewardedVideo;

		private bool FakeMenuActive {
			get => fakeMenuGameObject.activeSelf;
			set => fakeMenuGameObject.SetActive(value);
		}

		private bool RewardActive {
			get => rewardGameObject.activeSelf;
			set => rewardGameObject.SetActive(value);
		}

		protected virtual void Start() {

			FakeMenuActive = false;
			RewardActive = false;

			StartCoroutine(FakeLoad());
		}

		private IEnumerator FakeLoad() {

			progressText.text = "Loading...";
			yield return new WaitForSeconds(2.0f);
			progressText.text = string.Empty;

			PlayPack.RegisterOnline((duration) => {

				progressText.text = "Offline Time: " + duration.ToString("N0") + "s";
			});
		}

		protected virtual void Update() {

			privacyButtonGameObject.SetActive(PlayPack.CanShowPrivacySettings);
			surveyButtonGameObject.SetActive(PlayPack.CanShowSurvey);
			rvButtonGameObject.SetActive(CanOfferReward);
		}

		protected virtual void OnDestroy() {

			PlayPack.UnregisterOnline();
		}

		public void OnPrivacyButton() {

			PlayPack.ShowPrivacySettings();
		}

		public void OnSupportButton() {

			PlayPack.SendSupportEmail();
		}

		public void OnSurveyButton() {

			// Typically, these values are changed when the survey is offered (via a popup), not when the survey button is pressed.
			// This means that the popup will only show once a day for three days regardless of whether the button was pressed or the dialog dismissed.
			// Placing them here as there is no popup to offer a survey in the example project.
			PlayPack.SurveySeenDay = DayHelper.CurrentDay;
			PlayPack.SurveySeenCount++;

			PlayPack.OpenSurvey(() => {

				Debug.Log("Survey has been completed");
			});
		}

		public void OnMenuButton() {

			menuButton.interactable = false;
			FakeMenuActive = true;
		}

		public void OnFakeMenuButton() {

			FakeMenuActive = false;

			if (!PlayPack.ShowInterstitialAdvert(Done) && !PlayPack.ShowRateDialog(Done)) {

				Done();
			}

			void Done() {

				menuButton.interactable = true;
			}
		}

		public void OnRvButton() {

			rvButton.interactable = !PlayPack.ShowRewardedVideo(() => rvButton.interactable = true, (id, value) => {

				RewardActive = true;
			});
		}

		public void OnDebugButton() {

			PlayPack.ShowAdvertDebugger();
		}
	}
}
