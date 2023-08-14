using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NeonPlay.Newtonsoft.Json;
using NeonPlay.Newtonsoft.Json.Converters;
using NeonPlay.Newtonsoft.Json.Serialization;
using NeonPlay.Newtonsoft.Json.Utilities;
using UnityEngine;

namespace NeonPlay.Json {

	public static class JsonDotNetHelper {

		public class StringEnumerationConverter : StringEnumConverter {

		}

		public class JsonDotNetHelperObject : IJsonHelperObject {

			public string SerialiseObject(object jsonObject, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool indented = false) {

				return JsonDotNetHelper.SerialiseObject(jsonObject, includePrivateVariables, typeHandling, indented);
			}

			public TObject DeserialiseObject<TObject>(string json, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto) {

				return JsonDotNetHelper.DeserialiseObject<TObject>(json, includePrivateVariables, typeHandling);
			}
		}

		public static string SerialiseObject(object jsonObject, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool indented = false) {

			return JsonConvert.SerializeObject(jsonObject, indented ? Formatting.Indented : Formatting.None, CreateJsonSerializerSettings<CustomContractResolver>(includePrivateVariables, typeHandling));
		}

		private static TObject DeserialiseObject<TObject>(string json, JsonSerializerSettings jsonSerializerSettings, bool rethrowException) {

			try {

				return JsonConvert.DeserializeObject<TObject>(json, jsonSerializerSettings);

			} catch (Exception exception) {

				Debug.LogException(exception);

				if (rethrowException) {

					throw;

				} else {

					return default;
				}
			}
		}

		public static TObject DeserialiseObject<TObject>(string json, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool rethrowException = false) {

			return DeserialiseObject<TObject>(json, CreateJsonSerializerSettings<CustomContractResolver>(includePrivateVariables, typeHandling), rethrowException);
		}

		public static string SerialiseObject<TContractResolver>(object jsonObject, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool indented = false) where TContractResolver : DefaultContractResolver, new() {

			return JsonConvert.SerializeObject(jsonObject, indented ? Formatting.Indented : Formatting.None, CreateJsonSerializerSettings<TContractResolver>(includePrivateVariables, typeHandling));
		}

		public static TObject DeserialiseObject<TObject, TContractResolver>(string json, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool rethrowExcept = false) where TContractResolver : DefaultContractResolver, new() {

			return DeserialiseObject<TObject>(json, CreateJsonSerializerSettings<TContractResolver>(includePrivateVariables, typeHandling), rethrowExcept);
		}

		public static string SerialiseObject(object jsonObject, DefaultContractResolver contractResolver, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool indented = false) {

			return JsonConvert.SerializeObject(jsonObject, indented ? Formatting.Indented : Formatting.None, CreateJsonSerializerSettings(includePrivateVariables, typeHandling, contractResolver));
		}

		public static TObject DeserialiseObject<TObject>(string json, DefaultContractResolver contractResolver, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool rethrowException = false) {

			return DeserialiseObject<TObject>(json, CreateJsonSerializerSettings(includePrivateVariables, typeHandling, contractResolver), rethrowException);
		}

		public class CustomContractResolver : DefaultContractResolver {

			private HashSet<Type> optInSet = new HashSet<Type>();

			protected override JsonObjectContract CreateObjectContract(System.Type objectType) {

				SerialiseOptInAttribute optInAttribute = CachedAttributeGetter<SerialiseOptInAttribute>.GetAttribute(objectType);

				if (optInAttribute != null) {

					optInSet.Add(objectType);
				}

				return base.CreateObjectContract(objectType);
			}

			protected override bool ForceSerialisation(MemberInfo member) {

				return (JsonTypeReflector.GetAttribute<SerialiseIncludeAttribute>(member) != null);
			}

			protected virtual bool ForceIncludeProperty(JsonProperty property) {

				return false;
			}

			protected virtual bool ForceIgnoreProperty(JsonProperty property) {

				return false;
			}

			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {

				JsonProperty property = base.CreateProperty(member, memberSerialization);
				SerialiseIncludeAttribute includeAttribute = JsonTypeReflector.GetAttribute<SerialiseIncludeAttribute>(member);

				if ((includeAttribute != null) || ForceIncludeProperty(property)) {

					property.Readable = ReflectionUtils.CanReadMemberValue(member, true);
					property.Writable = ReflectionUtils.CanSetMemberValue(member, true, true);

					SerialiseNameAttribute nameAttribute = includeAttribute as SerialiseNameAttribute;
					if (nameAttribute != null) {

						property.PropertyName = nameAttribute.Name;
					}

					SerialiseEnumerationAsStringAttribute serialiseAsStringAttribute = JsonTypeReflector.GetAttribute<SerialiseEnumerationAsStringAttribute>(member);
					if (serialiseAsStringAttribute != null) {

						if (property.Converter != null) {

							throw new InvalidOperationException("Would override custom propery converter: " + property.Converter + ": because of attribute: " + serialiseAsStringAttribute);
						}

						StringEnumConverter memberConverter = new StringEnumConverter();

						memberConverter.LowerCaseText = serialiseAsStringAttribute.LowerCase;

						if (!memberConverter.CanConvert(property.PropertyType)) {

							throw new InvalidOperationException("Cannot convert: " + property.PropertyName + ": with StringEnumConverter");
						}

						property.Converter = memberConverter;

						if (!string.IsNullOrEmpty(serialiseAsStringAttribute.Name)) {

							property.PropertyName = serialiseAsStringAttribute.Name;
						}
					}

				} else {

					SerialiseIgnoreAttribute ignoreAttribute = JsonTypeReflector.GetAttribute<SerialiseIgnoreAttribute>(member);

					if ((ignoreAttribute != null) || ForceIgnoreProperty(property) || optInSet.Contains(member.DeclaringType)) {

						property.Ignored = true;
					}
				}

				return property;
			}
		}

		public class ScriptableObjectContractResolver : CustomContractResolver {

			protected override bool ForceIgnoreProperty(JsonProperty property) {

				string name = property.PropertyName;

				if ((name == "name") || (name == "hideFlags")) {

					return true;
				}

				return base.ForceIgnoreProperty(property);
			}
		}

		public class ScriptableObjectConverter : CustomCreationConverter<ScriptableObject> {

			public override ScriptableObject Create(Type objectType) {
				return ScriptableObject.CreateInstance(objectType);
			}
		}

		private static JsonSerializerSettings CreateJsonSerializerSettings<TContractResolver>(bool includePrivateVariable, JsonTypeHandling typeHandling) where TContractResolver : DefaultContractResolver, new() {

			return CreateJsonSerializerSettings(includePrivateVariable, typeHandling, new TContractResolver());
		}

		private static JsonSerializerSettings CreateJsonSerializerSettings(bool includePrivateVariable, JsonTypeHandling typeHandling, DefaultContractResolver contractResolver) {

			JsonSerializerSettings settings = new JsonSerializerSettings();
			JsonConverter[] converters = { new ScriptableObjectConverter() };

			switch (typeHandling) {
				case JsonTypeHandling.Auto:
					settings.TypeNameHandling = TypeNameHandling.Auto;
					settings.SpecialPropertyHandling = SpecialPropertyHandling.ReadAhead;
					break;

				case JsonTypeHandling.On:
					settings.TypeNameHandling = TypeNameHandling.All;
					settings.SpecialPropertyHandling = SpecialPropertyHandling.ReadAhead;
					break;

				default:
					break;
			}

			if (includePrivateVariable) {

				contractResolver.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
			}

			settings.Converters = converters;
			settings.ContractResolver = contractResolver;

			return settings;
		}
	}
}
