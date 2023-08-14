using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NeonPlay.EditorHelper {

	public static class EditorPlayerSettingsHelper {

		public static HashSet<string> GetScriptingDefines(BuildTargetGroup buildTargetGroup) {

			string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

			return new HashSet<string>(currentDefines.Split(';'));
		}

		public static void SetScriptingDefines(BuildTargetGroup buildTargetGroup, HashSet<string> defineSet) {

			HashSet<string> currentDefineSet = GetScriptingDefines(buildTargetGroup);

			if (!defineSet.SetEquals(currentDefineSet)) {

				string[] defineArray = defineSet.ToArray();

				Array.Sort(defineArray, Compare);

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defineArray));

				int Compare(string left, string right) {

					const string GameAnalyticsPrefix = "gameanalytics_";
					int leftPool = left.StartsWith(GameAnalyticsPrefix) ? 1 : 0;
					int rightPool = right.StartsWith(GameAnalyticsPrefix) ? 1 : 0;
					int poolOrder = leftPool.CompareTo(rightPool);

					if (poolOrder == 0) {

						return left.CompareTo(right);
					}

					return poolOrder;
				}
			}
		}

		public static void SetScriptingDefines(BuildTargetGroup buildTargetGroup, bool set, params string[] defineList) {

			if (set) {

				AddScriptingDefines(buildTargetGroup, defineList);

			} else {

				RemoveScriptingDefines(buildTargetGroup, defineList);
			}
		}

		public static void AddScriptingDefines(BuildTargetGroup buildTargetGroup, params string[] defineList) {

			HashSet<string> currentDefineSet = GetScriptingDefines(buildTargetGroup);

			foreach (string define in defineList) {

				currentDefineSet.Add(define);
			}

			SetScriptingDefines(buildTargetGroup, currentDefineSet);
		}

		public static void RemoveScriptingDefines(BuildTargetGroup buildTargetGroup, params string[] defineList) {

			HashSet<string> currentDefineSet = GetScriptingDefines(buildTargetGroup);

			foreach (string define in defineList) {

				currentDefineSet.Remove(define);
			}

			SetScriptingDefines(buildTargetGroup, currentDefineSet);
		}

		public static SerializedProperty GetProjectSettingsProperty(string propertyPath) {

			PlayerSettings playerSettings = AssetDatabase.LoadAssetAtPath<PlayerSettings>("ProjectSettings/ProjectSettings.asset");
			SerializedObject serialisedObject = new SerializedObject(playerSettings);

			return serialisedObject.FindProperty(propertyPath);
		}

		public static SerializedProperty GetUnityConnectSettingsProperty(string propertyPath) {

			Object unityConnectSettings = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/UnityConnectSettings.asset");
			SerializedObject serialisedObject = new SerializedObject(unityConnectSettings);

			return serialisedObject.FindProperty(propertyPath);
		}

		public static SerializedProperty GetUnityAdsSettingsProperty(string propertyPath) {

			SerializedProperty unityConnectProperty = GetUnityConnectSettingsProperty("UnityAdsSettings");

			return unityConnectProperty.FindPropertyRelative(propertyPath);
		}

		public static SerializedProperty GetUnityAnalyticsSettingsProperty(string propertyPath) {

			SerializedProperty unityConnectProperty = GetUnityConnectSettingsProperty("UnityAnalyticsSettings");

			return unityConnectProperty.FindPropertyRelative(propertyPath);
		}

		public static SerializedProperty GetCrashReportingSettingsProperty(string propertyPath) {

			SerializedProperty unityConnectProperty = GetUnityConnectSettingsProperty("CrashReportingSettings");

			return unityConnectProperty.FindPropertyRelative(propertyPath);
		}

		public static SerializedProperty GetUnityAdsPlatform(string platform) {

			SerializedProperty serialisedProperty = GetUnityAdsSettingsProperty("m_GameIds");

			for (int i = 0; i < serialisedProperty.arraySize; i++) {

				SerializedProperty entrySerialisedProperty = serialisedProperty.GetArrayElementAtIndex(i);
				SerializedProperty platformSerialisedProperty = entrySerialisedProperty.FindPropertyRelative("first");

				if (platformSerialisedProperty.stringValue == platform) {

					SerializedProperty idSerialisedProperty = entrySerialisedProperty.FindPropertyRelative("second");

					return idSerialisedProperty;
				}
			}

			serialisedProperty.InsertArrayElementAtIndex(0);

			SerializedProperty newEntrySerialisedProperty = serialisedProperty.GetArrayElementAtIndex(0);
			SerializedProperty newPlatformSerialisedProperty = newEntrySerialisedProperty.FindPropertyRelative("first");
			SerializedProperty newIdSerialisedProperty = newEntrySerialisedProperty.FindPropertyRelative("second");

			newPlatformSerialisedProperty.stringValue = platform;

			return newIdSerialisedProperty;
		}

		public static bool ProjectSettingsAvailable => AssetDatabase.LoadAssetAtPath<PlayerSettings>("ProjectSettings/ProjectSettings.asset") != null;
		public static SerializedProperty OrganisationIdProperty => GetProjectSettingsProperty("organizationId");
		public static SerializedProperty CloudProjectIdProperty => GetProjectSettingsProperty("cloudProjectId");
		public static SerializedProperty AndroidSplashImageProperty => GetProjectSettingsProperty("androidSplashScreen");
		public static SerializedProperty SupportedUrlSchemesProperty => GetProjectSettingsProperty("iOSURLSchemes");

		public static string UnityAdsAndroidId {
			get => GetUnityAdsPlatform("AndroidPlayer").stringValue;
			set {
				SerializedProperty serialisedProperty = GetUnityAdsPlatform("AndroidPlayer");

				serialisedProperty.stringValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static string UnityAdsIosId {
			get => GetUnityAdsPlatform("iPhonePlayer").stringValue;
			set {
				SerializedProperty serialisedProperty = GetUnityAdsPlatform("iPhonePlayer");

				serialisedProperty.stringValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static bool UnityAdsEnabled {
			get => GetUnityAdsSettingsProperty("m_Enabled").boolValue;
			set {
				SerializedProperty serialisedProperty = GetUnityAdsSettingsProperty("m_Enabled");

				serialisedProperty.boolValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static bool UnityAnalyticsEnabled {
			get => GetUnityAnalyticsSettingsProperty("m_Enabled").boolValue;
			set {
				SerializedProperty serialisedProperty = GetUnityAnalyticsSettingsProperty("m_Enabled");

				serialisedProperty.boolValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static bool CrashReportingEnabled {
			get => GetCrashReportingSettingsProperty("m_Enabled").boolValue;
			set {
				SerializedProperty serialisedProperty = GetCrashReportingSettingsProperty("m_Enabled");

				serialisedProperty.boolValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static string OrganisationId {
			get => OrganisationIdProperty.stringValue;
			set {
				SerializedProperty serialisedProperty = OrganisationIdProperty;

				serialisedProperty.stringValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static string CloudProjectId {
			get => CloudProjectIdProperty.stringValue;
			set {
				SerializedProperty serialisedProperty = CloudProjectIdProperty;

				serialisedProperty.stringValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static Texture2D AndroidSplashImage {
			get => (Texture2D) AndroidSplashImageProperty.objectReferenceValue;
			set {
				SerializedProperty serialisedProperty = AndroidSplashImageProperty;

				serialisedProperty.objectReferenceValue = value;
				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static HashSet<string> SupportedUrlSchemes {
			get {
				SerializedProperty serializedProperty = SupportedUrlSchemesProperty;
				int count = serializedProperty.arraySize;
				HashSet<string> set = new HashSet<string>();

				for (int i = 0; i < count; i++) {

					set.Add(serializedProperty.GetArrayElementAtIndex(i).stringValue);
				}

				return set;
			}
			set {
				SerializedProperty serialisedProperty = SupportedUrlSchemesProperty;
				int index = 0;

				serialisedProperty.ClearArray();

				foreach (string scheme in value) {

					if (!string.IsNullOrEmpty(scheme)) {

						serialisedProperty.InsertArrayElementAtIndex(index);
						SerializedProperty element = serialisedProperty.GetArrayElementAtIndex(index);

						element.stringValue = scheme;

						index++;
					}
				}

				serialisedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static void RemoveSupportedUrlSchemes(Regex regex) {

			HashSet<string> set = SupportedUrlSchemes;

			set.RemoveWhere(item => regex.IsMatch(item));
			SupportedUrlSchemes = set;
		}

		public static void IncludeSupportedUrlSchemes(string scheme) {

			HashSet<string> set = SupportedUrlSchemes;

			set.Add(scheme);
			SupportedUrlSchemes = set;
		}
	}
}
