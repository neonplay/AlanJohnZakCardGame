using System;

namespace NeonPlay {

	public static class UriBuilderExtensions {

		public static void AppendQuery(this UriBuilder uriBuilder, string query) {

			if ((uriBuilder.Query != null) && (uriBuilder.Query.Length > 1)) {

				uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + query;

			} else {

				uriBuilder.Query = query;
			}
		}

		public static void AppendQuery(this UriBuilder uriBuilder, string prefix, string value) {

			uriBuilder.AppendQuery(prefix + "=" + value);
		}
	}
}