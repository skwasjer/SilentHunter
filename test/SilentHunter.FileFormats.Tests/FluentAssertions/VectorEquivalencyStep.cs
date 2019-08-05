using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions.Equivalency;
using SilentHunter.FileFormats.Extensions;

namespace SilentHunter.FileFormats.FluentAssertions
{
	public class VectorEquivalencyStep<T> : IEquivalencyStep
		where T : struct
	{
		private readonly double _epsilon;

		public VectorEquivalencyStep(double epsilon = double.Epsilon)
		{
			_epsilon = epsilon;
		}

		public bool CanHandle(IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
		{
			// TODO: support also for non enumerable types, if needed.
			if (!context.RuntimeType.IsClosedTypeOf(typeof(IEnumerable<>)))
			{
				return false;
			}

			Type elementType = context.RuntimeType.GetElementType();
			return elementType == typeof(T);

		}

		public bool Handle(IEquivalencyValidationContext context, IEquivalencyValidator parent, IEquivalencyAssertionOptions config)
		{
			if (ReferenceEquals(context.Subject, context.Expectation))
			{
				return true;
			}

			FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
			if (!fieldInfos.All(fi => fi.FieldType == typeof(float) || fi.FieldType == typeof(double)))
			{
				throw new InvalidOperationException("Cannot compare vectors. One or more fields is not a float or double.");
			}

			List<T> vectors = ((IEnumerable<T>)context.Subject).ToList();
			List<T> expectedVectors = ((IEnumerable<T>)context.Expectation).ToList();

			if (vectors.Count != expectedVectors.Count)
			{
				return false;
			}

			for (int i = 0; i < vectors.Count; i++)
			{
				T left = vectors[i];
				T right = expectedVectors[i];

				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					double leftValue = Convert.ToDouble(fieldInfo.GetValue(left));
					double rightValue = Convert.ToDouble(fieldInfo.GetValue(right));

					if (!NearlyEqual(leftValue, rightValue, _epsilon))
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool NearlyEqual(double a, double b, double epsilon)
		{
			const double minNormal = 2.2250738585072014E-308d;
			double absA = Math.Abs(a);
			double absB = Math.Abs(b);
			double diff = Math.Abs(a - b);

			// Shortcut, handles infinities
			if (a.Equals(b))
			{
				return true;
			}

			if (a == 0 || b == 0 || absA + absB < minNormal)
			{
				// a or b is zero or both are extremely close to it
				// relative error is less meaningful here
				return diff < (epsilon * minNormal);
			}

			// use relative error
			return diff / (absA + absB) < epsilon;
		}
	}
}