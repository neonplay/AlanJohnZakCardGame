using NeonPlayUi;
using UnityEngine;
using UnityEngine.UI;

namespace NeonPlayExample {

	public class NeonPlayExampleRate : MonoBehaviour {

		[SerializeField] private RatePopup ratePopup;
		[SerializeField] private Button rateButton;

		public void OnRate() {

			rateButton.interactable = !ratePopup.Show(() => rateButton.interactable = true);
		}
	}
}
