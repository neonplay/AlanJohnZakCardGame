using NeonPlayHelper;
using System;
using UnityEngine;

namespace NeonPlayUi {

	public class RatePopup : MonoBehaviour {

		private const string KeyPrefix = "NeonPlayRatePopup";
		private const string AcceptedReviewKey = KeyPrefix + "AcceptedReview";

		[SerializeField] private GameObject content;
		[SerializeField] private GameObject bannerSpacer;

		private Action completionAction;

		public bool ShowingReview => content.activeSelf;

		public bool AcceptedReview {
			get => SaveGameHelper.GetBoolean(AcceptedReviewKey);
			private set => SaveGameHelper.SetBoolean(AcceptedReviewKey, value);
		}

		protected virtual void Start() {

			content.SetActive(false);
		}

		protected virtual void Update() {

			bannerSpacer.SetActive(AdvertHelper.BannerVisible);
		}

		public string GetRatePopupText(RatePopupText.Entry entry) {

			switch (entry) {
				case RatePopupText.Entry.Title:
					return "High Five!";
				case RatePopupText.Entry.Description:
					return "Loving " + Application.productName + "? Leave us a review!";
				case RatePopupText.Entry.Button:
					return "Review";
				default:
					return string.Empty;
			}
		}

		public bool Show(Action onComplete) {

			if (!AcceptedReview && !ShowingReview) {

				completionAction = onComplete;
				content.SetActive(true);

				return true;
			}

			return false;
		}

		private void Hide() {

			content.SetActive(false);
			completionAction?.Invoke();
			completionAction = null;
		}

		public void OnClose() {

			Hide();
		}

		public void OnReview() {

#if UNITY_IOS && !UNITY_EDITOR
			UnityEngine.iOS.Device.RequestStoreReview();
#elif UNITY_ANDROID && !UNITY_EDITOR
			PaperPlaneTools.RateBox.Instance.ForceShow();
#endif

			AcceptedReview = true;
			Hide();
		}
	}
}
