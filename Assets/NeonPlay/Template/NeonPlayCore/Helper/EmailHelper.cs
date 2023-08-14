using UnityEngine;

namespace NeonPlay.Helper {

	public static class EmailHelper {

		private static string EscapeText(string text) {

			if (!string.IsNullOrEmpty(text)) {

				return WWW.EscapeURL(text).Replace("+", "%20");
			}

			return string.Empty;
		}

		public static bool ValidateAddress(ref string address) {

			if (!string.IsNullOrEmpty(address)) {

				address = address.TrimStart(' ');
				address = address.TrimEnd(' ');

				// TODO: Full validation of the email address.
				return address.Contains("@") && !address.EndsWith("@") && !address.StartsWith("@") && !address.EndsWith(".");
			}

			return false;
		}

		public static bool SendEmail(string address, string subject, string body) {

			if (ValidateAddress(ref address)) {

				string escapedSubject = EscapeText(subject);
				string escapedBody = EscapeText(body);

				Application.OpenURL("mailto:" + address + "?subject=" + escapedSubject + "&body=" + escapedBody);

				return true;
			}

			return false;
		}
	}
}
