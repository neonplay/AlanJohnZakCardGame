using System;

namespace NeonPlay {

	public static class UriExtensions {

		public static bool GetQuery(this Uri uri, string prefix, out string value) {

			string query = uri.Query;

			if (query.StartsWith("?", StringComparison.Ordinal)) {

				string[] splitQuery = query.Substring(1).Split('&');
				string match = prefix + "=";
				string result = Array.Find(splitQuery, item => item.StartsWith(match, StringComparison.Ordinal));

				if (!string.IsNullOrEmpty(result)) {

					value = result.Substring(match.Length);

					return true;
				}
			}

			value = string.Empty;

			return false;
		}
	}
}