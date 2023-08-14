using TMPro;
using UnityEngine;

namespace NeonPlayUi {

	public abstract class NeonPlayUiText : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI text;

		protected TextMeshProUGUI Text => text;

		protected abstract void Start();

#if UNITY_EDITOR
		protected virtual void OnValidate() {

			if (text == null) {

				text = GetComponent<TextMeshProUGUI>();
			}
		}
#endif
	}
}
