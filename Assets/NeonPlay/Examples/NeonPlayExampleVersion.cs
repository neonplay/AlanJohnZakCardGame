using NeonPlayHelper;
using TMPro;
using UnityEngine;

namespace NeonPlayExample {

	public class NeonPlayExampleVersion : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI versionText;
		[SerializeField] private TextMeshProUGUI osVersionText;

		protected virtual void Start() {

			versionText.text = VersionHelper.VersionString;
			osVersionText.text = "OS: " + VersionHelper.OsVersionString;
		}
	}
}
