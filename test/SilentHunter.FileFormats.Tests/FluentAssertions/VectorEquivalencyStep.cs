using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions.Equivalency;

namespace SilentHunter.FileFormats.FluentAssertions;

public class VectorEquivalencyStep<T> : IEquivalencyStep
    where T : struct
{
    private readonly double _epsilon;

    public VectorEquivalencyStep(double epsilon = double.Epsilon)
    {
        _epsilon = epsilon;
    }

    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        if (ReferenceEquals(comparands.Subject, comparands.Expectation))
        {
            return EquivalencyResult.AssertionCompleted;
        }

        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
        if (!fieldInfos.All(fi => fi.FieldType == typeof(float) || fi.FieldType == typeof(double)))
        {
            return EquivalencyResult.ContinueWithNext;
            //throw new InvalidOperationException("Cannot compare vectors. One or more fields is not a float or double.");
        }

        if (!(comparands.Subject is IEnumerable<T> subject && comparands.Expectation is IEnumerable<T> expectation))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        var vectors = subject.ToList();
        var expectedVectors = expectation.ToList();

        if (vectors.Count != expectedVectors.Count)
        {
            return EquivalencyResult.ContinueWithNext;
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
                    return EquivalencyResult.ContinueWithNext;
                }
            }
        }

        return EquivalencyResult.AssertionCompleted;
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
            return diff < epsilon * minNormal;
        }

        // use relative error
        return diff / (absA + absB) < epsilon;
    }
}
