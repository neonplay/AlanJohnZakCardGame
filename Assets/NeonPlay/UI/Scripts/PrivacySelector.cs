using System;
using UnityEngine;

namespace NeonPlayUi {

	public class PrivacySelector : MonoBehaviour {

		public enum PrivacyPage {
			Welcome,
			Confirm,
			Settings,
			Information,
			Access,
			Ccpa
		}

		[Serializable]
		private class Page {

			public PrivacyPage privacyPage;
			public GameObject gameObject;
		}

		[SerializeField]
		private Page[] pageList;

		public PrivacyPage CurrentPage { get; private set; }

		public void SelectPage(PrivacyPage privacyPage) {

			CurrentPage = privacyPage;

			foreach (Page page in pageList) {

				page.gameObject.SetActive(page.privacyPage == privacyPage);
			}
		}
	}
}
