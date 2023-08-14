using System;
using UnityEngine;

namespace NeonPlay {

#if !USING_TEMPLATE
	public interface IDebugLogType {

	}

	public static class DebugLogging {

		public static void Log(string category, string message, UnityEngine.Object context) {

			Debug.Log(message, context);
		}

		public static void Log(string category, string message) {

			Debug.Log(message);
		}

		public static void LogWarning(string category, string message, UnityEngine.Object context) {

			Debug.LogWarning(message, context);
		}

		public static void LogWarning(string category, string message) {

			Debug.LogWarning(message);
		}

		public static void LogError(string category, string message, UnityEngine.Object context) {

			Debug.LogError(message, context);
		}

		public static void LogError(string category, string message) {

			Debug.LogError(message);
		}

		public static void LogException(Exception exception, UnityEngine.Object context) {

			Debug.LogException(exception, context);
		}

		public static void LogException(Exception exception) {

			Debug.LogException(exception);
		}

		public static void Log<TObject>(TObject objectToLog, string message) where TObject : IDebugLogType {

			Debug.Log(message);
		}

		public static void Log<TObject>(TObject objectToLog, string message, UnityEngine.Object context) where TObject : IDebugLogType {

			Debug.Log(message, context);
		}

		public static void LogWarning<TObject>(TObject objectToLog, string message) where TObject : IDebugLogType {

			Debug.LogWarning(message);
		}

		public static void LogWarning<TObject>(TObject objectToLog, string message, UnityEngine.Object context) where TObject : IDebugLogType {

			Debug.LogWarning(message, context);
		}

		public static void LogError<TObject>(TObject objectToLog, string message) where TObject : IDebugLogType {

			Debug.LogError(message);
		}

		public static void LogError<TObject>(TObject objectToLog, string message, UnityEngine.Object context) where TObject : IDebugLogType {

			Debug.LogError(message, context);
		}
	}
#endif
}
