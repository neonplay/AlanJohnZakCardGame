using NeonPlay;
using NeonPlay.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonPlayHelper {

	public static class SaveGameHelper {

		private const string KeyPrefix = "SaveGameHelper";
		private const string SaveGameKey = KeyPrefix + "SaveGame";
		private static bool savingEnabled = true;
		private static bool dirty;
		private static bool applicationPaused;
		private static Dictionary<string, object> map;

		private static void Activate() {

			if (map == null) {

				PauseHelper.Activate();

				map = JsonDotNetHelper.DeserialiseObject<Dictionary<string, object>>(PlayerPrefs.GetString(SaveGameKey)) ?? new Dictionary<string, object>();

				PauseHelper.OnPause += OnPause;
			}
		}

		public static bool HasKey(string key) {

			Activate();

			return map.ContainsKey(key);
		}

		public static void DeleteAll() {

			Activate();
			map.Clear();
			SetDirty();
		}

		public static void DeleteKey(string key) {

			Activate();
			map.Remove(key);
			SetDirty();
		}

		private static void OnPause(bool paused) {

			applicationPaused = paused;

			if (paused) {

				Save();
			}
		}

		private static void SetDirty() {

			dirty = true;

			if (applicationPaused) {

				Save();
			}
		}

		public static void Save() {

			Activate();

			if (dirty) {

				PlayerPrefs.SetString(SaveGameKey, JsonDotNetHelper.SerialiseObject(map));

				dirty = false;
			}
		}

		public static void Import(string saveGame) {

			Activate();

			map = JsonDotNetHelper.DeserialiseObject<Dictionary<string, object>>(saveGame) ?? new Dictionary<string, object>();

			PlayerPrefs.SetString(SaveGameKey, saveGame);
			PlayerPrefs.Save();
		}

		public static string Export() {

			Activate();

			return JsonDotNetHelper.SerialiseObject(map);
		}

		public static void DisableSaving() {

			savingEnabled = false;
		}

		private static void SetNullable<TType>(string key, TType? value, Action<string, TType> set) where TType : struct {

			if (savingEnabled) {

				if (value.HasValue) {

					set(key, value.Value);

				} else {

					DeleteKey(key);
				}
			}
		}

		private static TType? GetNullable<TType>(string key, Func<string, TType, TType> get) where TType : struct {

			return HasKey(key) ? get(key, default) : (TType?)null;
		}

		private static TType GetOrSet<TType>(string key, TType value, Func<string, TType, TType> get, Action<string, TType> set) {

			if (HasKey(key)) {

				return get(key, value);
			}

			set(key, value);

			return value;
		}

		private static void Set<TType>(string key, TType value) {

			Activate();

			if (savingEnabled) {

				map[key] = value;
				SetDirty();
			}
		}

		private static bool IsIntegerNumber(object value) {

			return value is sbyte
				|| value is byte
				|| value is short
				|| value is ushort
				|| value is int
				|| value is uint
				|| value is long
				|| value is ulong;
		}

		private static bool IsRealNumber(object value) {

			return value is float
				|| value is double
				|| value is decimal;
		}

		private static int Get(string key, int defaultValue) {

			if (map.TryGetValue(key, out object objectValue)) {

				if (IsIntegerNumber(objectValue)) {

					return Convert.ToInt32(objectValue);

				} else {

					Debug.LogWarning("Get: " + key + ": Reading integer from a key of type: " + objectValue.GetType().Name);
				}
			}

			return defaultValue;
		}

		private static float Get(string key, float defaultValue) {

			if (map.TryGetValue(key, out object objectValue)) {

				if (IsRealNumber(objectValue)) {

					return Convert.ToSingle(objectValue);

				} else {

					Debug.LogWarning("Get: " + key + ": Reading float from a key of type: " + objectValue.GetType().Name);
				}
			}

			return defaultValue;
		}

		private static TType Get<TType>(string key, TType defaultValue) {

			Activate();

			if (map.TryGetValue(key, out object value)) {

				return (TType)value;
			}

			return defaultValue;
		}

		public static void SetBoolean(string key, bool value) {

			Set(key, value);
		}

		public static bool GetBoolean(string key, bool defaultValue = false) {

			return Get(key, defaultValue);
		}

		public static void SetNullableBoolean(string key, bool? value) {

			SetNullable(key, value, SetBoolean);
		}

		public static bool? GetNullableBoolean(string key) {

			return GetNullable<bool>(key, GetBoolean);
		}

		public static bool GetOrSetBoolen(string key, bool value) {

			return GetOrSet(key, value, GetBoolean, SetBoolean);
		}

		public static void SetString(string key, string value) {

			Set(key, value);
		}

		public static string GetString(string key, string defaultValue = "") {

			return Get(key, defaultValue);
		}

		public static string GetOrSetString(string key, string value) {

			return GetOrSet(key, value, GetString, SetString);
		}

		public static void SetInt(string key, int value) {

			Set(key, value);
		}

		public static int GetInt(string key, int defaultValue = 0) {

			return Get(key, defaultValue);
		}

		public static void SetNullableInt(string key, int? value) {

			SetNullable(key, value, SetInt);
		}

		public static int? GetNullableInt(string key) {

			return GetNullable<int>(key, GetInt);
		}

		public static int GetOrSetInt(string key, int value) {

			return GetOrSet(key, value, GetInt, SetInt);
		}

		public static void SetFloat(string key, float value) {

			Set(key, value);
		}

		public static float GetFloat(string key, float defaultValue = 0.0f) {

			return Get(key, defaultValue);
		}

		public static void SetNullableFloat(string key, float? value) {

			SetNullable(key, value, SetFloat);
		}

		public static float? GetNullableFloat(string key) {

			return GetNullable<float>(key, GetFloat);
		}

		public static float GetOrSetFloat(string key, float value) {

			return GetOrSet(key, value, GetFloat, SetFloat);
		}

		public static void SetDouble(string key, double value) {

			Set(key, value);
		}

		public static double GetDouble(string key, double defaultValue = 0.0) {

			return Get(key, defaultValue);
		}

		public static void SetNullableDouble(string key, double? value) {

			SetNullable(key, value, SetDouble);
		}

		public static double? GetNullableDouble(string key) {

			return GetNullable<double>(key, GetDouble);
		}

		public static double GetOrSetDouble(string key, double value) {

			return GetOrSet(key, value, GetDouble, SetDouble);
		}

		public static void SetLong(string key, long value) {

			Set(key, value);
		}

		public static long GetLong(string key, long defaultValue = 0) {

			return Get(key, defaultValue);
		}

		public static void SetNullableLong(string key, long? value) {

			SetNullable(key, value, SetLong);
		}

		public static long? GetNullableLong(string key) {

			return GetNullable<long>(key, GetLong);
		}

		public static long GetOrSetLong(string key, long value) {

			return GetOrSet(key, value, GetLong, SetLong);
		}

		public static void SetEnumeration<TEnum>(string key, TEnum value) where TEnum : struct {

			if (savingEnabled) {

				SetString(key, value.ToString());
			}
		}

		public static TEnum GetEnumeration<TEnum>(string key, TEnum defaultValue = default) where TEnum : struct {

			if (Enum.TryParse(GetString(key), out TEnum result)) {

				return result;
			}

			return defaultValue;
		}

		public static void SetNullableEnumeration<TEnum>(string key, TEnum? value) where TEnum : struct {

			SetNullable(key, value, SetEnumeration);
		}

		public static TEnum? GetNullableEnumeration<TEnum>(string key) where TEnum : struct {

			return GetNullable<TEnum>(key, GetEnumeration);
		}

		public static TEnum GetOrSetEnumeration<TEnum>(string key, TEnum value) where TEnum : struct {

			return GetOrSet(key, value, GetEnumeration, SetEnumeration);
		}

		public static void SetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> value) {

			Set(key, value);
		}

		public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> defaultValue = null) {

			return Get(key, defaultValue);
		}

		public static Dictionary<TKey, TValue> GetOrSetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> value) {

			return GetOrSet(key, value, GetDictionary, SetDictionary);
		}

		public static void SetHashSet<TKey>(string key, HashSet<TKey> value) {

			Set(key, value);
		}

		public static HashSet<TKey> GetHashSet<TKey>(string key, HashSet<TKey> defaultValue = null) {

			return Get(key, defaultValue);
		}

		public static HashSet<TKey> GetOrSetHashSet<TKey>(string key, HashSet<TKey> value) {

			return GetOrSet(key, value, GetHashSet, SetHashSet);
		}
	}
}
