using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonPlayHelper {

	public static class AbHelper {

		private const string ConfigurationPath = "Configuration/NeonPlayConfiguration";
		private const string KeyPrefix = "AbHelper";
		private const string ModeKey = KeyPrefix + "Mode";

		private static bool initialised;

		private static NeonPlayConfiguration configuration;
		private static NeonPlayConfiguration Configuration {
			get {
				if (configuration == null) {

					configuration = Resources.Load<NeonPlayConfiguration>(ConfigurationPath);
				}

				return configuration;
			}
		}

		private static NeonPlayConfiguration.Ab AbConfiguration => Configuration.AbConfiguration;

		private static IReadOnlyList<string> modeList;
		public static IReadOnlyList<string> ModeList {
			get {
				if (modeList == null) {

					List<string> list = new List<string>();

					list.Add("Default Pool");
					list.Add(ControlModeName);
					list.AddRange(AbConfiguration.ModeList.Select(item => AbConfiguration.AbPhase + " " + item));

					modeList = list;
				}

				return modeList;
			}
		}

		private static string ControlModeName => AbConfiguration.AbPhase + " Control";

		public static string Mode {
			get {
				if (!initialised) {

					throw new InvalidOperationException("Must call Initialise() first");
				}

				return SaveGameHelper.GetString(ModeKey);
			}
		}

		private static string modeHash;
		public static string ModeHash {
			get {
				if (modeHash == null) {

					string mode = Mode;
					string phasePrefix = AbConfiguration.AbPhase.ToString() + " ";

					if ((mode != ControlModeName) && mode.StartsWith(phasePrefix)) {

						modeHash = GetHash(mode.Substring(phasePrefix.Length));
					}
				}

				return modeHash;
			}
		}

		public static string ModeHashSuffix {
			get {
				if (string.IsNullOrEmpty(ModeHash)) {

					return string.Empty;
				}

				return "." + ModeHash;
			}
		}

		private static string GetHash(string input) {

			using (MD5 md5 = MD5.Create()) {

				byte[] md5Bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

				return BitConverter.ToString(md5Bytes).Replace("-", "").Substring(0, 6).ToUpper();
			}
		}

		private static string SelectMode() {

			int individualPercentage = AbConfiguration.IndividualPercentage;
			int count = ModeList.Count;
			int total = Mathf.Max(100, individualPercentage * (count - 1));
			int random = Random.Range(0, total);

			for (int i = 1; i < count; i++, random -= individualPercentage) {

				if (random < individualPercentage) {

					return ModeList[i];
				}
			}

			return ModeList[0];
		}

		public static void Initialise() {

			if (!initialised) {

				if (!SaveGameHelper.HasKey(ModeKey)) {

					SaveGameHelper.SetString(ModeKey, SelectMode());
				}

				initialised = true;
			}
		}

		public static void DebugSetMode(string modeName) {

			SaveGameHelper.SetString(ModeKey, AbConfiguration.AbPhase + " " + modeName);
		}

		public static void DebugResetMode() {

			SaveGameHelper.DeleteKey(ModeKey);
		}

		public static bool GetActiveMode(out string modeName) {

			if (initialised) {

				string mode = Mode;
				string phasePrefix = AbConfiguration.AbPhase.ToString() + " ";

				if ((mode != ControlModeName) && mode.StartsWith(phasePrefix)) {

					modeName = mode.Substring(phasePrefix.Length);
					return true;
				}
			}

			modeName = string.Empty;
			return false;
		}
	}
}
