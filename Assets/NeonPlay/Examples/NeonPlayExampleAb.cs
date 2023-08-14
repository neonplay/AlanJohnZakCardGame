using NeonPlayHelper;
using TMPro;
using UnityEngine;

namespace NeonPlayExample {

	public class NeonPlayExampleAb : MonoBehaviour {

		[SerializeField]
		private TextMeshProUGUI abText;

		protected virtual void Start() {

			AbHelper.Initialise();
			AnalyticsHelper.Initialise();
			AnalyticsHelper.SetAbMode(AbHelper.Mode);

			if (AbHelper.GetActiveMode(out string modeName)) {

				abText.text = "A/B Mode: " + modeName;

			} else {

				abText.text = "No A/B Active (" + AbHelper.Mode + ")";
			}
		}
	}
}
