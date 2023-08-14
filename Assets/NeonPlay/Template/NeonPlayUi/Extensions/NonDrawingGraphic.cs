using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeonPlay.Ui {

    public class NonDrawingGraphic : Graphic {

		[SerializeField]
		private List<RectTransform> holeList;

        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }

		public void AddHole(RectTransform hole) {

			holeList.Add(hole);
		}

		public void RemoveHole(RectTransform hole) {

			holeList.RemoveAll(item => item == hole);
		}

		public override bool Raycast(Vector2 sp, Camera eventCamera) {

			foreach (RectTransform hole in holeList) {

				if (RectTransformUtility.RectangleContainsScreenPoint(hole, sp, eventCamera)) {

					if ((eventCamera == null) || (eventCamera.WorldToScreenPoint(hole.position).z <= eventCamera.farClipPlane)) {

						return false;
					}
				}
			}

			bool result = base.Raycast(sp, eventCamera);

			return result;
		}
	}
}
