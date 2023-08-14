using UnityEngine;

namespace NeonPlayExample {

	public class PlayPackExampleReward : MonoBehaviour {

		public void OnEnd() {

			gameObject.SetActive(false);
		}
	}
}
