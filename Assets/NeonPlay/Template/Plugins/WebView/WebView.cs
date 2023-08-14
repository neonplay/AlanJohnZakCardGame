using NeonPlay;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Template {

	public class WebView : MonoBehaviour, IDebugLogType {

		public enum Orientation {
			Unspecified = -1,
			Landscape = 0,
			Portrait = 1
		}

		public event Action<string> HasOpened;
		public event Action<string> HasClosed;
		public event Action<string> StoreHasOpened;
		public event Action<string> MailToHasOpened;

		public static WebView OpenWebView(string url, string title, string closeButtonText, bool displayTitleBar = true, bool allowScroll = true, Orientation orientation = Orientation.Unspecified, bool interceptMailTo = false, string deepLinkScheme = "") { 

#if USING_WEBVIEW
#if UNITY_EDITOR || UNITY_STANDALONE

			Application.OpenURL(url);
			return null;

#else

			GameObject updateObject = new GameObject("WebView");
			WebView webView = updateObject.AddComponent<WebView>();

			DebugLogging.Log(webView, "OpenWebView: URL: " + url + ": Title: " + title + ": CloseButtonText: " + closeButtonText + ": DisplayTitleBar: " + displayTitleBar + ": AllowScroll: " + allowScroll + ": Orientation: " + orientation + "InterceptMailTo: " + interceptMailTo + ": DeepLinkScheme: " + deepLinkScheme);

#if UNITY_IOS

			_OpenWebView(url, title, displayTitleBar, allowScroll, interceptMailTo, deepLinkScheme);

#elif UNITY_ANDROID

			_OpenWebView(url, title, closeButtonText, displayTitleBar, orientation, interceptMailTo, deepLinkScheme);

#endif
			DebugLogging.Log(webView, "OpenWebView: Opened");
	
			return webView;
#endif
#else
			return null;
#endif
		}

		public void Close() {

#if USING_WEBVIEW
#if !UNITY_EDITOR && !UNITY_STANDALONE
#if UNITY_IOS

			_CloseWebView();

#elif UNITY_ANDROID

			_CloseWebView();

#endif
#endif
#endif
		}

#if USING_WEBVIEW
#if !UNITY_EDITOR && !UNITY_STANDALONE
#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern void _OpenWebView(string url, string title, bool displayNavBar, bool allowScroll, bool interceptMailTo, string deepLinkScheme);
		[DllImport("__Internal")]
		private static extern void _CloseWebView();

#elif UNITY_ANDROID

		private static void _OpenWebView(string url, string title, string closeButtonText, bool displayTitleBar, Orientation orientation, bool interceptMailTo, string deepLinkScheme) {

			AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass wvActivity = new AndroidJavaClass("com.neonplay.webviewplugin.WebView");
			wvActivity.CallStatic("show", unityPlayerActivity, url, title, closeButtonText, displayTitleBar ? "default" : "notitle", (int) orientation, interceptMailTo, deepLinkScheme);
		}

		private static void _CloseWebView() {

			AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass wvActivity = new AndroidJavaClass("com.neonplay.webviewplugin.WebView");
			wvActivity.CallStatic("close");
		}

#endif

		private void Awake() {

			DebugLogging.Log(this, "Awake");
		}

		private void OnOpen(string parameter) {

			DebugLogging.Log(this, "OnOpen: parameter: " + parameter);

			HasOpened?.Invoke(parameter);
		}

		private void OnClose(string parameter) {

			DebugLogging.Log(this, "OnClose: parameter: " + parameter);

			HasClosed?.Invoke(parameter);

			Destroy(gameObject);
		}

		private void OnStore(string parameter) {

			DebugLogging.Log(this, "OnStore: parameter: " + parameter);
	
			StoreHasOpened?.Invoke(parameter);
		}

		private void OnMailTo(string parameter) {

			DebugLogging.Log(this, "OnMailTo: parameter: " + parameter);
	
			MailToHasOpened?.Invoke(parameter);
		}
#endif
#endif
	}
}
