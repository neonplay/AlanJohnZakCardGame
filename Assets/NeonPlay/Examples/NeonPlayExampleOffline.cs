using NeonPlayHelper;
using System.Collections;
using TMPro;
using UnityEngine;

namespace NeonPlayExample {

	public class NeonPlayExampleOffline : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI offlineTimeText;

		protected virtual void Start() {

			offlineTimeText.text = "";

			OfflineHelper.MinimumOfflineDuration = 10.0;
			OfflineHelper.MaximumOfflineDuration = 60.0;

			OfflineHelper.OfflineHasEnded += duration => {

				offlineTimeText.text = "Offline Time: " + duration.ToString("N2");

				StartCoroutine(Clear());

				IEnumerator Clear() {

					yield return new WaitForSeconds(5.0f);

					offlineTimeText.text = "";
				}
			};

			// Only activate the offline helper after the OfflineHasEnded event has been set
			// (by any system the code that uses this action) to ensure the initial event is
			// not missed as it is called by Activate.
			OfflineHelper.Activate();
		}
	}
}
