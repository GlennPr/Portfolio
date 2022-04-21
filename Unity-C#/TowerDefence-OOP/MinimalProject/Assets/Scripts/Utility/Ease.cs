using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
	public class Ease
	{
		public enum EaseType
		{
			OutQuad,
			InOutSine,
			InOutCubic,
			InOutQuint,
			InOutCirc,
			InOutElastic
		}

		public static float ApplyEase(float number, EaseType ease)
		{
			switch (ease)
			{
				case EaseType.OutQuad:
					return EaseOutQuad(number);
				case EaseType.InOutSine:
					return EaseInOutSine(number);
				case EaseType.InOutCubic:
					return EaseInOutCubic(number);
				case EaseType.InOutQuint:
					return EaseInOutQuint(number);
				case EaseType.InOutCirc:
					return EaseInOutCirc(number);
				case EaseType.InOutElastic:
					return EaseInOutElastic(number);
			}

			return number;
		}

		public static float EaseOutQuad(float value)
		{
			return 1f - (1 - value) * (1 - value);
		}

		public static float EaseInOutSine(float value)
		{
			return -(Mathf.Cos(Mathf.PI * value) - 1) / 2f;
		}
		public static float EaseInOutCubic(float value)
		{
			return value < 0.5f ? 4 * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 3) / 2f;
		}
		public static float EaseInOutQuint(float value)
		{
			return value < 0.5f ? 16f * value * value * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 5) / 2f;
		}
		public static float EaseInOutCirc(float value)
		{
			return value < 0.5f
				? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * value, 2))) / 2f
				: (Mathf.Sqrt(1 - Mathf.Pow(-2 * value + 2, 2)) + 1) / 2f;
		}

		public static float EaseInOutElastic(float value)
		{
			float c5 = (2 * Mathf.PI) / 4.5f;

			return value == 0
				? 0
				: Mathf.Approximately(value, 1)
					? 1
					: value < 0.5f
						? -(Mathf.Pow(2, 20 * value - 10) * Mathf.Sin((20 * value - 11.125f) * c5)) / 2f
						: (Mathf.Pow(2, -20 * value + 10) * Mathf.Sin((20 * value - 11.125f) * c5)) / 2f + 1;
		}
	}
}
