using System.Runtime.InteropServices;
using UnityEngine;

namespace NeonPlayHelper {

	public static class RegionHelper {

#if UNITY_IOS
		[DllImport("__Internal")]
		extern static private string LocaleGetRegion();
		[DllImport("__Internal")]
		extern static private string LocaleGetLanguage();
#elif UNITY_ANDROID
		private static AndroidJavaObject androidLocaleObject;
		private static AndroidJavaObject AndroidLocale {
			get {
				if (androidLocaleObject == null) {
					AndroidJavaClass androidLocaleClass = new AndroidJavaClass("java.util.Locale");
					androidLocaleObject = androidLocaleClass.CallStatic<AndroidJavaObject>("getDefault");
				}
				return androidLocaleObject;
			}
		}
		private static string LocaleGetRegion() {
			return AndroidLocale.Call<string>("getCountry");
		}
		private static string LocaleGetLanguage() {
			return AndroidLocale.Call<string>("getLanguage");
		}
#endif

		public static string Region {
			get {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
				try {

					return LocaleGetRegion().ToUpper();

				} catch {

					return DefaultRegion;
				}
#else
				return DefaultRegion;
#endif
			}
		}

		public static string DefaultRegion => "GB";
		public static bool EuRegion => IsEuRegion(Region);
		public static bool GdprRegion => IsGdprRegion(Region);
		public static bool CcpaRegion => IsCcpaRegion(Region);

		public static bool IsGdprRegion(string region) {

			return IsDpa18Region(region) || IsEeaRegion(region);
		}

		public static bool IsCcpaRegion(string region) {

			switch (region.ToUpper()) {
				case "US":
					return true;
				default:
					return false;
			}
		}

		public static bool IsDpa18Region(string region) {

			switch (region.ToUpper()) {
				case "GB": // United Kingdom of Great Britain and Northern Ireland
					return true;
				default:
					return false;
			}
		}

		public static bool IsEeaRegion(string region) {

			switch (region.ToUpper()) {
				case "IS": // Iceland
				case "LI": // Liechtenstein
				case "NO": // Norway
					return true;
				default:
					return IsEuRegion(region);
			}
		}

		public static bool IsEuRegion(string region) {

			switch (region.ToUpper()) {
				case "AT": // Austria
				case "BE": // Belgium
				case "BG": // Bulgaria
				case "HR": // Croatia
				case "CY": // Cyprus
				case "CZ": // Czechia
				case "DK": // Denmark
				case "EE": // Estonia
				case "FI": // Finland
				case "FR": // France
				case "DE": // Germany
				case "GR": // Greece
				case "HU": // Hungary
				case "IE": // Ireland
				case "IT": // Italy
				case "LV": // Latvia
				case "LT": // Lithuania
				case "LU": // Luxembourg
				case "MT": // Malta
				case "NL": // Netherlands
				case "PL": // Poland
				case "PT": // Portugal
				case "RO": // Romania
				case "SK": // Slovakia
				case "SI": // Slovenia
				case "ES": // Spain
				case "SE": // Sweden
					return true;
				default:
					return false;
			}
		}
	}
}
