using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace NeonPlay.Ui {

	[CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
	public class NonDrawingGraphicEditor : GraphicEditor {

		protected SerializedProperty holeListProperty;

		protected override void OnEnable() {

			base.OnEnable();

			holeListProperty = serializedObject.FindProperty("holeList");
		}

		public override void OnInspectorGUI() {

			serializedObject.Update();

			EditorGUILayout.PropertyField(m_Script);
			EditorGUILayout.PropertyField(holeListProperty, true);

			RaycastControlsGUI();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
