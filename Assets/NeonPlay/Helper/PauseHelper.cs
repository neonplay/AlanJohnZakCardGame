using System;
using UnityEngine;

namespace NeonPlayHelper {

	public class PauseHelper : MonoBehaviour {

		private bool previousPauseState;

		private static PauseHelper instance;
		public static event Action<bool> OnPause;

		public static void Activate() {

			if (instance == null) {

				GameObject gameObject = new GameObject("PauseHelper");

				instance = gameObject.AddComponent<PauseHelper>();
				DontDestroyOnLoad(gameObject);
			}
		}

		protected virtual void OnApplicationQuit() {

			if (!previousPauseState) {

				OnPause?.Invoke(true);

				previousPauseState = true;
			}
		}

		protected virtual void OnApplicationPause(bool paused) {

			if (previousPauseState != paused) {

				OnPause.Invoke(paused);

				previousPauseState = paused;
			}
		}
	}
}
