using TMPro;
using UnityEngine;

namespace NeonPlayUi {

	public class RatePopupText : NeonPlayUiText {

		public enum Entry {
			Title,
			Description,
			Button
		}

		[SerializeField]
		private Entry entry;

		[SerializeField]
		private RatePopup ratePopup;

		protected override void Start() {

			Text.text = ratePopup.GetRatePopupText(entry);
		}

#if UNITY_EDITOR
		protected override void OnValidate() {

			base.OnValidate();

			if (ratePopup == null) {

				ratePopup = GetComponentInParent<RatePopup>();
			}
		}
#endif
	}
}
