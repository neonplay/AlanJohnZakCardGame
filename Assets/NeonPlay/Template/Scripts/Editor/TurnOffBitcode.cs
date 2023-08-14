#if UNITY_IOS

#if UNITY_2019_3_OR_NEWER

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Template {

	public class TurnOffBitcode : IPostprocessBuildWithReport {

		public int callbackOrder => 0;

		public void OnPostprocessBuild(BuildReport report) {

#if UNITY_IOS
			string pbxPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
			PBXProject project = new PBXProject();

			project.ReadFromFile(pbxPath);

			string mainTargetGuid = project.GetUnityMainTargetGuid();
			string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

			project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
			project.SetBuildProperty(frameworkTargetGuid, "ENABLE_BITCODE", "NO");
			project.WriteToFile(pbxPath);
#endif
		}
	}
}

#else

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Template {

	public static class TurnOffBitcode {

		[PostProcessBuild(999)]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {

			if (buildTarget == BuildTarget.iOS) {

				string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

				PBXProject pbxProject = new PBXProject();
				pbxProject.ReadFromFile(projectPath);

				string target = pbxProject.TargetGuidByName("Unity-iPhone");            
				pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

				pbxProject.WriteToFile (projectPath);
			}
		}
	}
}

#endif
#endif
