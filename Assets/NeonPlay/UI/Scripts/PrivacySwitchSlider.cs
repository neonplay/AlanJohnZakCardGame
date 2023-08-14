using UnityEngine;
using UnityEngine.UI;

namespace NeonPlayUi {

	public class PrivacySwitchSlider : MonoBehaviour {

		[SerializeField]
		private RectTransform slider;

		[SerializeField]
		private AspectRatioFitter fillFitter;

		[SerializeField]
		private Vector2 sliderOffset;

		[SerializeField]
		private float fillRatio = 1.0f;

		[SerializeField]
		private float slideDuration = 0.25f;

		private Vector2 sliderPosition;
		private float fillAspectRatio;
		private float sliderAlpha;

		public bool Value { get; set; }

		protected virtual void Update() {

			float deltaTime = Time.unscaledDeltaTime;

			sliderAlpha += (Value ? deltaTime : -deltaTime) * (1.0f / slideDuration);
			sliderAlpha = Mathf.Clamp01(sliderAlpha);

			if (slider != null) {

				slider.anchoredPosition = sliderPosition + slider.sizeDelta * sliderOffset * sliderAlpha;
			}

			if (fillFitter != null) {

				fillFitter.aspectRatio = Mathf.Lerp(fillAspectRatio, fillRatio, sliderAlpha);
			}
		}
	}
}
