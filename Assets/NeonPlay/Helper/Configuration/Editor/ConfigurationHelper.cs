using NeonPlayUi;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NeonPlayHelper.Editor {

	[InitializeOnLoad]
	public static class ConfigurationHelper {

		private const string AssetFolderPath = "Assets";
		private const string ResourcesFolderName = "Resources";
		private const string ResourcesFolderPath = AssetFolderPath + "/" + ResourcesFolderName;
		private const string ConfigurationFolderName = "Configuration";
		private const string ConfigurationFolderPath = ResourcesFolderPath + "/" + ConfigurationFolderName;
		private const string ConfigurationAssetName = "NeonPlayConfiguration.asset";
		private const string ConfigurationAssetPath = ConfigurationFolderPath + "/" + ConfigurationAssetName;
		private const string GameAnalyticsSettingsFolderName = "GameAnalytics";
		private const string GameAnalyticsSettingsFolderPath = ResourcesFolderPath + "/" + GameAnalyticsSettingsFolderName;
		private const string GameAnalyticsSettingsAssetName = "Settings.asset";
		private const string GameAnalyticsSettingsAssetPath = GameAnalyticsSettingsFolderPath + "/" + GameAnalyticsSettingsAssetName;
		private const string NeonPlayFolderName = "NeonPlay";
		private const string NeonPlayFolderPath = AssetFolderPath + "/" + NeonPlayFolderName;
		private const string NeonPlayUiFolderName = "UI";
		private const string NeonPlayUiFolderPath = NeonPlayFolderPath + "/" + NeonPlayUiFolderName;
		private const string NeonPlayUiPrefabsFolderName = "Prefabs";
		private const string NeonPlayUiPrefabsFolderPath = NeonPlayUiFolderPath + "/" + NeonPlayUiPrefabsFolderName;
		private const string NeonPlayPrivacyCanvasName = "NeonPlayPrivacyCanvas.prefab";
		private const string NeonPlayPrivacyCanvasPath = NeonPlayUiPrefabsFolderPath + "/" + NeonPlayPrivacyCanvasName;
		private const string NeonPlayRatePopupCanvasName = "NeonPlayRatePopupCanvas.prefab";
		private const string NeonPlayRatePopupCanvasPath = NeonPlayUiPrefabsFolderPath + "/" + NeonPlayRatePopupCanvasName;

		static ConfigurationHelper() {

			ApplyConfigurationChanges();
		}

		private static void CreateFolder(string parentFolder, string newFolderName) {

			if (!AssetDatabase.IsValidFolder(parentFolder + "/" + newFolderName)) {

				AssetDatabase.CreateFolder(parentFolder, newFolderName);
			}
		}

		[MenuItem("Neon Play Helper/Open Configuration...")]
		private static void OpenConfiguration() {

			Selection.activeObject = LoadOrCreateConfiguration();
		}

		[MenuItem("Neon Play Helper/Apply Configuration Changes")]
		private static void ApplyConfigurationChanges() {

			NeonPlayConfiguration configuration = AssetDatabase.LoadAssetAtPath<NeonPlayConfiguration>(ConfigurationAssetPath);

			if (configuration != null) {

				SetupGameAnalytics(configuration);
				SetupAppLovinMax(configuration);
			}
		}

		public static NeonPlayConfiguration LoadOrCreateConfiguration() {

			NeonPlayConfiguration configuration = AssetDatabase.LoadAssetAtPath<NeonPlayConfiguration>(ConfigurationAssetPath);

			if (configuration == null) {

				CreateFolder(AssetFolderPath, ResourcesFolderName);
				CreateFolder(ResourcesFolderPath, ConfigurationFolderName);
				AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<NeonPlayConfiguration>(), ConfigurationAssetPath);
				configuration = AssetDatabase.LoadAssetAtPath<NeonPlayConfiguration>(ConfigurationAssetPath);
			}

			SerializedObject serialisedObject = new SerializedObject(configuration);
			SerializedProperty privacyScreenProperty = serialisedObject.FindProperty("playPackConfiguration.privacyScreen");
			SerializedProperty ratePopupProperty = serialisedObject.FindProperty("playPackConfiguration.ratePopup");

			if (privacyScreenProperty.objectReferenceValue == null) {

				privacyScreenProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<PrivacyScreen>(NeonPlayPrivacyCanvasPath);
			}

			if (ratePopupProperty.objectReferenceValue == null) {

				ratePopupProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<RatePopup>(NeonPlayRatePopupCanvasPath);
			}

			serialisedObject.ApplyModifiedPropertiesWithoutUndo();

			return configuration;
		}

		private static void SetupGameAnalytics(NeonPlayConfiguration configuration) {

			NeonPlayConfiguration.GameAnalytics gameAnalyticsConfiguration = configuration.GameAnalyticsConfiguration;
			NeonPlayConfiguration.Ab abConfiguration = configuration.AbConfiguration;

			GameAnalyticsSDK.Setup.Settings gameAnalyticsSettings = AssetDatabase.LoadAssetAtPath<GameAnalyticsSDK.Setup.Settings>(GameAnalyticsSettingsAssetPath);

			if (gameAnalyticsSettings != null) {

				if (!gameAnalyticsSettings.Platforms.Contains(RuntimePlatform.IPhonePlayer)) {

					gameAnalyticsSettings.AddPlatform(RuntimePlatform.IPhonePlayer);
				}

				if (!gameAnalyticsSettings.Platforms.Contains(RuntimePlatform.Android)) {

					gameAnalyticsSettings.AddPlatform(RuntimePlatform.Android);
				}

				int iosIndex = gameAnalyticsSettings.Platforms.FindIndex(item => item == RuntimePlatform.IPhonePlayer);
				int androidIndex = gameAnalyticsSettings.Platforms.FindIndex(item => item == RuntimePlatform.Android);

				gameAnalyticsSettings.Build[iosIndex] = PlayerSettings.bundleVersion;
				gameAnalyticsSettings.Build[androidIndex] = PlayerSettings.bundleVersion;

				InvokeUpdateKey(gameAnalyticsSettings.UpdateGameKey, iosIndex, gameAnalyticsConfiguration.IosGameKey);
				InvokeUpdateKey(gameAnalyticsSettings.UpdateSecretKey, iosIndex, gameAnalyticsConfiguration.IosSecretKey);
				InvokeUpdateKey(gameAnalyticsSettings.UpdateGameKey, androidIndex, gameAnalyticsConfiguration.AndroidGameKey);
				InvokeUpdateKey(gameAnalyticsSettings.UpdateSecretKey, androidIndex, gameAnalyticsConfiguration.AndroidSecretKey);

				List<string> experimentList = new List<string>();

				experimentList.Add("Default Pool");
				experimentList.Add(abConfiguration.AbPhase + " Control");
				experimentList.AddRange(abConfiguration.ModeList.Select(item => abConfiguration.AbPhase + " " + item));

				gameAnalyticsSettings.CustomDimensions01 = experimentList;
				gameAnalyticsSettings.SubmitFpsAverage = true;

				EditorUtility.SetDirty(gameAnalyticsSettings);

				void InvokeUpdateKey(Action<int, string> updateAction, int index, string value) {

					if (!string.IsNullOrEmpty(value)) {

						updateAction(index, value);
					}
				}
			}
		}

		public static void SetupAppLovinMax(NeonPlayConfiguration configuration) {

			string iosAdMobAppId = configuration.AdvertConfiguration.IosIdSet.AdMobId;
			string androidAdMobAppId = configuration.AdvertConfiguration.AndroidIdSet.AdMobId;
			string settingsPath = "Assets/NeonPlay/MaxSdk/Resources/AppLovinSettings.asset";
			AppLovinSettings settingsAsset = AssetDatabase.LoadAssetAtPath<AppLovinSettings>(settingsPath);
			SerializedObject serialisedObject = new SerializedObject(settingsAsset);
			SerializedProperty sdkKeyProperty = serialisedObject.FindProperty("sdkKey");
			SerializedProperty adMobIosAppIdProperty = serialisedObject.FindProperty("adMobIosAppId");
			SerializedProperty adMobAndroidAppIdProperty = serialisedObject.FindProperty("adMobAndroidAppId");

			AssignProperty(sdkKeyProperty, AdvertHelper.AppLovinMaxSdkKey);
			AssignProperty(adMobIosAppIdProperty, iosAdMobAppId);
			AssignProperty(adMobAndroidAppIdProperty, androidAdMobAppId);

			serialisedObject.ApplyModifiedPropertiesWithoutUndo();

			void AssignProperty(SerializedProperty serializedProperty, string value) {

				if (!string.IsNullOrEmpty(value)) {

					serializedProperty.stringValue = value;
				}
			}
		}
	}
}
