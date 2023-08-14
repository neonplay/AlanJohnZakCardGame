using UnityEditor;
using UnityEngine;

namespace NeonPlayHelper.Editor {

	public static class UpdateHelper {

		[MenuItem("Neon Play Helper/Delete SDK Folders")]
		private static void DeleteSdkFolders() {

			if (EditorUtility.DisplayDialog("Delete SDK Folders", "This will delete the SDK folders that are added with the NeonPlayExternal Unity package file. It will remove any modified or added files (such as settings).\n\nPlease ensure all files are committed to source control before proceeding", "OK", "Cancel")) {

				AssetDatabase.DeleteAsset("Assets/ExternalDependencyManager");
				AssetDatabase.DeleteAsset("Assets/NeonPlay");
				AssetDatabase.DeleteAsset("Assets/Plugins/Android/MaxMediationGoogle.androidlib");
				AssetDatabase.DeleteAsset("Assets/Plugins/iOS/Firebase");
			}
		}

		[MenuItem("Neon Play Helper/Check Project")]
		private static void CheckProject() {

			bool hasError = false;

			if (!AssetDatabase.IsValidFolder("Assets/GameAnalytics")) {

				EditorUtility.DisplayDialog("Check Project", "GameAnalyics appears to be missing from the project. This is required by the NeonPlayExternal Unity package, but is not provided with it.", "OK");
				hasError = true;
			}

			if (!AssetDatabase.IsValidFolder("Assets/FacebookSDK")) {

				EditorUtility.DisplayDialog("Check Project", "The Facebook SDK appears to be missing from the project. This is required by the NeonPlayExternal Unity package, but is not provided with it.", "OK");
				hasError = true;
			}

			if (!hasError) {

				EditorUtility.DisplayDialog("Check Project", "No problems detected.\n\nEnsure that the project is pushed to version control and use the Neon Play Helper > Delete SDK Folders option before importing the Play Pack Unity package.", "OK");
			}
		}

		[MenuItem("Neon Play Helper/Switch to iOS", true)]
		private static bool SwitchToIosValidate() {

			return EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS;
		}

		[MenuItem("Neon Play Helper/Switch to iOS")]
		private static void SwitchToIos() {

			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
		}

		private static string VersionPath = "Assets/NeonPlay/Helper/Editor/Version.asset";

		public static string Version {
			get {
				string version = "Unknown";
				TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(VersionPath);

				if (textAsset != null) {

					version = textAsset.text;
				}

				return version;
			}
			set {
				AssetDatabase.DeleteAsset(VersionPath);
				AssetDatabase.CreateAsset(new TextAsset(value), VersionPath);
			}
		}

		[MenuItem("Neon Play Helper/About...", priority = 100)]
		private static void About() {

			EditorUtility.DisplayDialog("Neon Play Helper", "Version: " + Version, "OK");
		}
	}
}
