using NeonPlay.Helper;
using NeonPlayHelper;
using UnityEngine;

namespace NeonPlayExample {

	public class NeonPlayExampleSurvey : MonoBehaviour {

		[SerializeField]
		private string surveyUrl;

		[SerializeField]
		private string surveyKey;

		[SerializeField]
		private GameObject surveyButton;

		public void Start() {

			SurveyHelper.Setup(IdHelper.PlayerId, IdHelper.DeepLinkId, surveyKey);

			if (SurveyHelper.SurveyRewardGiven) {

				surveyButton.SetActive(false);

			} else {

				SurveyHelper.SurveyHasCompleted += OnSurveyComplete;
			}
		}

		public void OnDestroy() {

			SurveyHelper.SurveyHasCompleted -= OnSurveyComplete;
		}

		private void OnSurveyComplete() {

			surveyButton.SetActive(false);
		}

		public void OnSurveyButton() {

			SurveyHelper.Open(IdHelper.PlayerId, IdHelper.DeepLinkId, surveyUrl);
		}
	}
}
