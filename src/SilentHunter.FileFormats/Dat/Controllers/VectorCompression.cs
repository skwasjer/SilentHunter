using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using SilentHunter.Controllers;

namespace SilentHunter.FileFormats.Dat.Controllers
{
	public static class VectorCompression
	{
		/// <summary>
		/// Decompresses the compressed vectors into an enumerable of <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <returns>An enumerable of <typeparamref name="T" />.</returns>
		public static IEnumerable<T> Decompress<T>(this CompressedVectors compressedVectors)
			where T : struct
		{
			CheckIfSupportedVectorType(typeof(T));

			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < compressedVectors.Vectors.Count; i += fields.Length)
			{
				// Box value type.
				var inst = (object)new T();
				for (int j = 0; j < fields.Length; j++)
				{
					float val = compressedVectors.Scale * compressedVectors.Vectors[i + j] + compressedVectors.Translation;
					fields[j].SetValue(inst, val);
				}

				yield return (T)inst;
			}
		}


		/// <summary>
		/// Compresses vectors into an array where each float component of each vector is represented by an Int16 value, based one scale and translation.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <param name="vectors"></param>
		/// <param name="padToCount">Optional argument, to pad the result array of vectors to. This is useful for animation sequences where each animation frame requires the same amount of vectors, but should otherwise be omitted.</param>
		/// <returns>A <see cref="CompressedVectors" /> with the compressed vectors.</returns>
		public static CompressedVectors Compress<T>(ICollection<T> vectors, int padToCount = -1)
			where T : struct
		{
			CheckIfSupportedVectorType(typeof(T));

			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);

			GetCompressionRange(vectors, out float bias, out float scale);

			var compressedMeshTransform = new CompressedVectors
			{
				Scale = scale,
				Translation = bias,
				Vectors = new List<short>(padToCount > 0 ? padToCount * fields.Length : 0)
			};

			foreach (float floatValue in vectors
					.SelectMany(v => fields
						.Select(f => (float)f.GetValue(v))
					)
				)
			{
				compressedMeshTransform.Vectors.Add(
					(short)Math.Round(
						((double)floatValue - bias) / scale
					)
				);
			}

			// Add extra zeroes, in case a fixed length of vectors is required. Animation sequences require each frame to have the same number of vectors. Note however, that adding zeroes will break the mesh animation.
			// This is an edge case that's simply as a fix to prevent crashes, but is not an actual solution and should typically be handled externally.
			if (padToCount > 0)
			{
				for (int i = 0; i < (padToCount - vectors.Count) * fields.Length; i++)
				{
					compressedMeshTransform.Vectors.Add(0);
				}
			}

			return compressedMeshTransform;
		}

		// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
		private static void CheckIfSupportedVectorType(Type type)
		{
			if (type != typeof(Vector2) && type != typeof(Vector3))
			{
				throw new NotSupportedException("Expected Vector2 or Vector3");
			}
		}

		/// <summary>
		/// Gets the best possible compression factors required (least amount of loss of precision) to align compressed vectors into Int16 values.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <param name="vectors">The vectors to determine compression factors for.</param>
		/// <param name="bias">The bias for each vector component.</param>
		/// <param name="scale">The scaling factor for each vector component.</param>
		private static void GetCompressionRange<T>(IEnumerable<T> vectors, out float bias, out float scale)
			where T : struct
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);

			float fmax = 0, fmin = 0;

			// We need highest and lowest possible floats from all vectors.
			foreach (float floatValue in vectors
					.SelectMany(v => fields
						.Select(f => (float)f.GetValue(v))
					)
				)
			{
				fmax = Math.Max(floatValue, fmax);
				fmin = Math.Min(floatValue, fmin);
			}

			// Using min/max, determine scale and bias. We want the fmax value to result into the highest Int16 value, and the fmin value to result into the lowest Int16 value.
			bias = 0f;
			scale = (fmax - fmin) / ushort.MaxValue;

			if (fmax / scale > short.MaxValue)
			{
				bias = (fmax / scale - short.MaxValue) * scale;
			}
			else if (fmin / scale < short.MinValue)
			{
				bias = (fmin / scale - short.MinValue) * scale;
			}
		}
	}
}
