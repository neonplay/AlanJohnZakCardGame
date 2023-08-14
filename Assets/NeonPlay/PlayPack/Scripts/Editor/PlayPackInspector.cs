using NeonPlayHelper;
using NeonPlayHelper.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace NeonPlay {

	[CustomEditor(typeof(PlayPack))]
	public class PlayPackInspector : Editor {

		private SerializedObject configurationSerialisedObject;

		protected virtual void OnEnable() {

			NeonPlayConfiguration neonPlayConfiguration = ConfigurationHelper.LoadOrCreateConfiguration();

			configurationSerialisedObject = new SerializedObject(neonPlayConfiguration);
		}

		public override void OnInspectorGUI() {

			base.OnInspectorGUI();

			if (configurationSerialisedObject != null) {

				EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

				EditorGUI.indentLevel++;
				ObjectPropertyField(configurationSerialisedObject, item => item.name != "m_Script");
				EditorGUI.indentLevel--;
			}
		}

		private void ObjectPropertyField(SerializedObject serializedObject, Func<SerializedProperty, bool> predicate = null) {

			serializedObject.Update();

			SerializedProperty childProperty = serializedObject.GetIterator();

			for (childProperty.Next(true); childProperty.NextVisible(false);) {

				if ((predicate == null) || predicate(childProperty)) {

					EditorGUILayout.PropertyField(childProperty, true);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
