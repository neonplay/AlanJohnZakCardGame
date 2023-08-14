using NeonPlay;
using System;
using System.Security.Cryptography;
using System.Text;
using Template;
using UnityEngine;

namespace NeonPlayHelper {

	public static class SurveyHelper {

		private const string SurveyHelperRewardGivenKey = "SurveyHelper_RewardGiven";

		public static event Action SurveyHasCompleted;

		public static bool SurveyRewardGiven {
			get => SaveGameHelper.GetBoolean(SurveyHelperRewardGivenKey);
			private set => SaveGameHelper.SetBoolean(SurveyHelperRewardGivenKey, value);
		}

		public static void Setup(string playerId, string deepLinkScheme, string surveyKey) {

			Debug.Log("SurveyHelper.Setup: playerId: " + playerId + ": deepLinkScheme: " + deepLinkScheme + ": surveyKey: " + surveyKey);

			InitialiserHelper.RegisterDeepLinkAction(OnDeepLink);

			void OnDeepLink(Uri uri) {

				Debug.Log("SurveyHelper.Setup: OnDeepLink: " + uri + ": Host: " + uri.Host + ": LocalPath: " + uri.LocalPath + ": HasToken: " + uri.GetQuery("token", out string debugToken) + ": Token: " + debugToken);

				if ((uri.Scheme == deepLinkScheme) && (uri.Host == "survey") && (uri.LocalPath == "/complete") && uri.GetQuery("token", out string token)) {

					Debug.Log("SetupSurveyReward: OnDeepLink: Got survey reward deep link: Given: " + SurveyRewardGiven);

					if (!SurveyRewardGiven) {

						string AesEncrypt(string input, string keyIn) {

							byte[] inputByteList = Encoding.UTF8.GetBytes(input);
							byte[] keyByteList = Encoding.ASCII.GetBytes(keyIn);
							RijndaelManaged aesManaged = new RijndaelManaged();

							aesManaged.Mode = CipherMode.ECB;
							aesManaged.Padding = PaddingMode.PKCS7;
							aesManaged.KeySize = 256;

							using (ICryptoTransform encryptor = aesManaged.CreateEncryptor(keyByteList, null)) {

								byte[] outputByteList = encryptor.TransformFinalBlock(inputByteList, 0, inputByteList.Length);

								return BitConverter.ToString(outputByteList).Replace("-", string.Empty);
							}
						}

						string playerIdAes = AesEncrypt(playerId, surveyKey);

						Debug.Log("SetupSurveyReward: OnDeepLink: playerIdAes: " + playerIdAes + ": Token: " + token);

						if (playerIdAes.ToLower() == token.ToLower()) {

							SurveyRewardGiven = true;
							SurveyHasCompleted?.Invoke();
						}
					}
				}
			}
		}

		public static bool Open(string playerId, string deepLinkScheme, string surveyUrl, Action onClosed = null) {

			UriBuilder uriBuilder = new UriBuilder(surveyUrl);

			uriBuilder.AppendQuery("playerid", playerId);

			string url = uriBuilder.Uri.AbsoluteUri;

			Debug.Log("SurveyHelper.Open: Opening URL: " + url);

			WebView webView = WebView.OpenWebView(url, Application.productName, "Close", false, deepLinkScheme: deepLinkScheme);

			if (webView != null) {

				Debug.Log("SurveyHelper.Open: Webview opened");

				webView.HasClosed += OnClosed;
				SurveyHasCompleted += OnSurveyCompleted;

				return true;

				void OnClosed(string parameter) {

					Debug.Log("SetupSurvey: OnSurvey: OnClosed");

					SurveyHasCompleted -= OnSurveyCompleted;
					webView.HasClosed -= OnClosed;

					onClosed?.Invoke();
				}

				void OnSurveyCompleted() {

					Debug.Log("SetupSurvey: OnSurvey: OnSurveyCompleted");

					if (webView != null) {

						webView.Close();
					}
				}
			}

			return false;
		}
	}
}
