using System;
using UnityEngine;

namespace NeonPlay.Helper {

	public static class MathfHelper {

		public static float Max(float a, float b) {

			return Mathf.Max(a, b);
		}

		public static float Min(float a, float b) {

			return Mathf.Min(a, b);
		}

		public static double Max(double a, double b) {

			return (!(a > b)) ? b : a;
		}

		public static double Min(double a, double b) {

			return (!(a < b)) ? b : a;
		}

		public static double Abs(double d) {

			return Math.Abs(d);
		}

		public static double Clamp01(double value) {

			if (value < 0.0) {

				return 0.0;
			}

			if (value > 1.0) {

				return 1.0;
			}

			return value;
		}

		public static double Lerp(double a, double b, double t) {

			return a + (b - a) * Clamp01(t);
		}
	}
}
