using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace NeonPlayHelper.Editor {

	public class BuildHelper : IPreprocessBuildWithReport, IPostprocessBuildWithReport {

		public int callbackOrder => 0;
		public UnityEngine.Object FacebookSettingsAsset => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/FacebookSDK/SDK/Resources/FacebookSettings.asset");

		public void OnPreprocessBuild(BuildReport report) {

			SetupScriptingDefines(report);
			SetupGoogleVersionHandler(report);
			SetupFacebook(report);
		}

		public void OnPostprocessBuild(BuildReport report) {

			WriteSingularFrameworks(report);
		}

		private void SetupScriptingDefines(BuildReport report) {

			ScriptingDefineHelper.SetupBuild(report.summary.platformGroup);
		}

		private void SetupFacebook(BuildReport report) {

			// Singular will log the install so prevent duplicate events by disabling Facebook's own tracking
			EnableFacebookAutoLogging(false);
		}

		private void EnableFacebookAutoLogging(bool enabled = true) {

			UnityEngine.Object asset = FacebookSettingsAsset;

			if (asset != null) {

				SerializedObject serialisedObject = new SerializedObject(asset);
				SerializedProperty loggingProperty = serialisedObject.FindProperty("autoLogAppEventsEnabled");

				if (loggingProperty != null) {

					loggingProperty.boolValue = enabled;
					serialisedObject.ApplyModifiedProperties();
				}
			}

#if UNITY_ANDROID
			Facebook.Unity.Editor.ManifestMod.GenerateManifest();
#endif
		}

		private void SetupGoogleVersionHandler(BuildReport report) {

#if UNITY_IOS
			Google.IOSResolver.PodfileAddUseFrameworks = true;
			Google.IOSResolver.PodfileStaticLinkFrameworks = true;
#endif
		}

		private void WriteSingularFrameworks(BuildReport report) {

#if UNITY_IOS
			string pbxPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
			PBXProject project = new PBXProject();

			project.ReadFromFile(pbxPath);

			string targetGuid = project.GetUnityFrameworkTargetGuid();
			project.AddFrameworkToProject(targetGuid, "Security.framework", false);
			project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
			project.AddFrameworkToProject(targetGuid, "iAd.framework", false);
			project.AddFrameworkToProject(targetGuid, "AdSupport.framework", false);
			project.AddFrameworkToProject(targetGuid, "WebKit.framework", false);
			project.AddFrameworkToProject(targetGuid, "libsqlite3.0.tbd", false);
			project.AddFrameworkToProject(targetGuid, "libz.tbd", false);
			project.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
			project.AddFrameworkToProject(targetGuid, "AdServices.framework", true);

            System.IO.File.WriteAllText(pbxPath, project.WriteToString());
#endif
		}

	}
}
