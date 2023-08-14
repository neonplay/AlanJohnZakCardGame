using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonPlayHelper {

	public class InitialiserHelper : MonoBehaviour {

		private static InitialiserHelper instance;

		private event Action<Uri> OnDeepLink;
		private List<Uri> deepLinkList;

		private static InitialiserHelper Create() {

			if (instance == null) {

				GameObject gameObject = new GameObject("Initialiser");

				instance = gameObject.AddComponent<InitialiserHelper>();
				DontDestroyOnLoad(gameObject);
			}

			return instance;
		}

		private void Awake() {

			deepLinkList = new List<Uri>();
		}

		public void RegisterDeviceDeepLink(string url) {

			if (!string.IsNullOrEmpty(url)) {

				RegisterDeepLink(url);
			}
		}

		public virtual void RegisterDeepLink(string url) {

			try {

				Uri uri = new Uri(url);

				InvokeDeepLinkAction(uri);

			} catch (Exception exception) {

				Debug.LogException(exception);
			}
		}

		private void InvokeDeepLinkAction(Uri uri) {

			deepLinkList.Add(uri);
			OnDeepLink?.Invoke(uri);
		}

		private void RegisterDeepLinkActionInternal(Action<Uri> action) {

			foreach (Uri deepLink in deepLinkList) {

				action(deepLink);
			}

			OnDeepLink += action;
		}

		private void UnregisterDeepLinkActionInternal(Action<Uri> action) {

			OnDeepLink -= action;
		}

		public static void RegisterDeepLinkAction(Action<Uri> action) {

			Create().RegisterDeepLinkActionInternal(action);
		}

		public static void UnregisterDeepLinkAction(Action<Uri> action) {

			Create().UnregisterDeepLinkActionInternal(action);
		}
	}
}
