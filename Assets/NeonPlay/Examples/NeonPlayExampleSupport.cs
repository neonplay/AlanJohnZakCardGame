using NeonPlay.Helper;
using UnityEngine;

namespace NeonPlayExample {

    public class NeonPlayExampleSupport : MonoBehaviour {

		[SerializeField] private string supportEmailAddress = "games@neonplay.com";

		public void OnSupportButton() {

			if (!string.IsNullOrEmpty(supportEmailAddress)) {

				string address = supportEmailAddress;
				string subject = "Help with " + Application.productName;

				EmailHelper.SendEmail(address, subject, string.Empty);
			}
		}
	}
}
