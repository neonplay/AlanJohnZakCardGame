using NeonPlay.EditorHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NeonPlayHelper {

	public static class ScriptingDefineHelper {

		public interface IDependency {

			bool Check(BuildTargetGroup buildTargetGroup);
		}

		public class ScriptingDefineDependency : IDependency {

			public readonly string Define;
			public readonly bool Fail;

			public ScriptingDefineDependency(string scriptingDefine) {

				Fail = scriptingDefine.StartsWith("!");
				Define = Fail ? scriptingDefine.Substring(1) : scriptingDefine;
			}

			public bool Check(BuildTargetGroup buildTargetGroup) {

				return EditorPlayerSettingsHelper.GetScriptingDefines(buildTargetGroup).Contains(Define) != Fail;
			}
		}

		public class FileDependency : IDependency {

			public readonly string RootDirectory;
			public readonly string Filename;
			public readonly bool Fail;

			public FileDependency(string rootDirectory, string childFilename, bool failMatch = false) {

				RootDirectory = rootDirectory;
				Filename = childFilename;
				Fail = failMatch;
			}

			public virtual bool Check(BuildTargetGroup buildTargetGroup) {

				string[] fileList;

				return Check(out fileList);
			}

			protected bool Check(out string[] fileList) {

				if (Directory.Exists(RootDirectory)) {

					fileList = Directory.GetFiles(RootDirectory, Filename, SearchOption.AllDirectories);

					return (fileList.Length > 0) != Fail;
				}

				fileList = null;

				return Fail;
			}
		}

		public class PackageDependency : IDependency {

			private readonly string PackageName;
			private readonly Version MinimumVersion;
			private readonly Version MaximumVersion;
			public readonly bool Fail;

			public PackageDependency(string packageName, Version minimumVersion = null, Version maximumVersion = null, bool failMatch = false) {

				PackageName = packageName;
				MinimumVersion = minimumVersion;
				MaximumVersion = maximumVersion;
				Fail = failMatch;
			}

			public bool Check(BuildTargetGroup buildTargetGroup) {

				string path = Path.GetDirectoryName(Application.dataPath) + Path.DirectorySeparatorChar + "Packages" + Path.DirectorySeparatorChar + "manifest.json";
				string json;

				try {

					json = File.ReadAllText(path);
					Dictionary<string, object> outerMap = NeonPlay.Json.JsonDotNetHelper.DeserialiseObject<Dictionary<string, object>>(json);
					NeonPlay.Newtonsoft.Json.Linq.JObject dependencyMapObject = (NeonPlay.Newtonsoft.Json.Linq.JObject)outerMap["dependencies"];
					Dictionary<string, string> dependencyMap = dependencyMapObject.ToObject<Dictionary<string, string>>();

					if (dependencyMap.TryGetValue(PackageName, out string versionString)) {

						try {

							Version versionNumber = new Version(versionString);

							return (((MinimumVersion == null) || (versionNumber > MinimumVersion)) && ((MaximumVersion == null) || (versionNumber <= MaximumVersion))) != Fail;

						} catch (ArgumentException) {

							return !Fail;
						}
					}

					return Fail;

				} catch {

					return Fail;
				}
			}
		}

		public class FilePropertyDependency : FileDependency {

			public readonly string Path;
			public readonly string StringValue;
			public readonly bool? BooleanValue;
			public readonly Enum EnumerationValue;
			public readonly Enum[] EnumerationValueList;

			public FilePropertyDependency(string rootDirectory, string childFilename, string propertyPath, string propertyValue, bool failMatch = false) : base(rootDirectory, childFilename, failMatch) {

				Path = propertyPath;
				StringValue = propertyValue;
			}

			public FilePropertyDependency(string rootDirectory, string childFilename, string propertyPath, Enum propertyValue, bool failMatch = false) : base(rootDirectory, childFilename, failMatch) {

				Path = propertyPath;
				EnumerationValue = propertyValue;
			}

			public FilePropertyDependency(string rootDirectory, string childFilename, string propertyPath, bool failMatch = false, params Enum[] propertyValueList) : base(rootDirectory, childFilename, failMatch) {

				Path = propertyPath;
				EnumerationValueList = propertyValueList;
			}

			public FilePropertyDependency(string rootDirectory, string childFilename, string propertyPath, bool propertyValue) : base(rootDirectory, childFilename) {

				Path = propertyPath;
				BooleanValue = propertyValue;
			}

			public override bool Check(BuildTargetGroup buildTargetGroup) {

				string[] fileList;

				if (base.Check(out fileList)) {

					bool propertyFound = false;

					foreach (string file in fileList) {

						UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(SystemToAssetPath(file));

						if (asset != null) {

							SerializedObject serialisedObject = new SerializedObject(asset);
							SerializedProperty serialisedProperty = serialisedObject.FindProperty(Path);

							if (serialisedProperty != null) {

								bool match = false;

								if (BooleanValue.HasValue) {

									match = serialisedProperty.boolValue == BooleanValue.Value;

								} else if (EnumerationValue != null) {

									match = serialisedProperty.enumValueIndex == Convert.ToInt32(EnumerationValue);

								} else if (EnumerationValueList != null) {

									match = Array.FindIndex(EnumerationValueList, item => serialisedProperty.enumValueIndex == Convert.ToInt32(item)) >= 0;

								} else if (StringValue != null) {

									match = serialisedProperty.stringValue == StringValue;

								} else {

									throw new NotImplementedException();
								}

								if (Fail != match) {

									propertyFound = true;
								}
							}
						}
					}

					return propertyFound;
				}

				return false;
			}
		}

		[MenuItem("Neon Play Helper/Setup Scripting Defines")]
		private static void SetupScriptingDefines() {

			Setup();
		}

		private static string SystemToAssetPath(string systemPath) {

			string[] splitPath = systemPath.Split(Path.DirectorySeparatorChar);
			string[] splitDataPath = Application.dataPath.Split('/');

			if (splitPath.Length >= splitDataPath.Length) {

				int count = splitDataPath.Length;

				for (int i = 0; i < count; i++) {

					if (splitPath[i].ToLower() != splitDataPath[i].ToLower()) {

						throw new ArgumentException("Specified path does not match Unity's data path");
					}
				}

			} else {

				throw new ArgumentException("Specified system path is too short");
			}

			int start = splitDataPath.Length;
			int length = splitPath.Length - start;
			string[] subPath = new string[length + 1];

			subPath[0] = "Assets";
			Array.Copy(splitPath, start, subPath, 1, length);

			return string.Join("/", subPath);
		}

		public static void SetScriptingDefine(string setDefine, bool invert, params IDependency[] withDependencyList) {

			SetScriptingDefine(BuildTargetGroup.iOS, setDefine, invert, withDependencyList);
			SetScriptingDefine(BuildTargetGroup.Android, setDefine, invert, withDependencyList);
			SetScriptingDefine(BuildTargetGroup.Standalone, setDefine, invert, withDependencyList);
		}

		public static void SetScriptingDefine(BuildTargetGroup buildTargetGroup, string setDefine, bool invert, params IDependency[] withDependencyList) {

			bool dependencyFound = true;

			foreach (IDependency withDependency in withDependencyList) {

				dependencyFound = dependencyFound && withDependency.Check(buildTargetGroup);
			}

			if (dependencyFound != invert) {

				EditorPlayerSettingsHelper.AddScriptingDefines(buildTargetGroup, setDefine);

			} else {

				EditorPlayerSettingsHelper.RemoveScriptingDefines(buildTargetGroup, setDefine);
			}
		}

		public static void Setup() {

			SetScriptingDefine("UNITY_PURCHASING", false, new PackageDependency("com.unity.purchasing", new Version("3.0.0")));
			SetScriptingDefine("USING_WEBVIEW", false, new FileDependency("Assets/Template/Plugins/WebView", "WebView.cs"));
		}

		public static void SetupBuild(BuildTargetGroup buildTargetGroup) {

			SetScriptingDefine(buildTargetGroup, "UNITY_PURCHASING", false, new PackageDependency("com.unity.purchasing", new Version("3.0.0")));
			SetScriptingDefine(buildTargetGroup, "USING_WEBVIEW", false, new FileDependency("Assets/Template/Plugins/WebView", "WebView.cs"));
		}
	}
}
