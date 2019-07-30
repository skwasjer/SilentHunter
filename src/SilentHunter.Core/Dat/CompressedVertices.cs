using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents a list of compressed vertices.
	/// </summary>
	public class CompressedVertices
	{
		/// <summary>
		/// The scale factor.
		/// </summary>
		public float Scale;

		/// <summary>
		/// The translation factor.
		/// </summary>
		public float Translation;

		/// <summary>
		/// The list of compressed vertices.
		/// </summary>
		public List<short> Vertices;

		/// <summary>
		/// Decompresses the compressed vertices into an enumerable of <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <returns>An enumerable of <typeparamref name="T" />.</returns>
		public IEnumerable<T> Decompress<T>()
			where T : struct
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
			for (var i = 0; i < Vertices.Count; i += fields.Length)
			{
				// Box value type.
				var inst = (object)new T();
				for (var j = 0; j < fields.Length; j++)
				{
					float val = Scale * Vertices[i + j] + Translation;
					fields[j].SetValue(inst, val);
				}

				yield return (T)inst;
			}
		}

		/// <summary>
		/// Compresses vertices into an array where each float component of each vertex is represented by an Int16 value.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <param name="vertices"></param>
		/// <param name="padToCount">Optional argument, to pad the result array of vertices to. This is useful for animation sequences where each animation frame requires the same amount of vertices, but should otherwise be omitted.</param>
		/// <returns>A <see cref="CompressedVertices" /> with the compressed vertices.</returns>
		public static CompressedVertices Compress<T>(ICollection<T> vertices, int padToCount = -1)
			where T : struct
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);

			GetCompressionRange(vertices, out float bias, out float scale);

			var compressedMeshTransform = new CompressedVertices
			{
				Scale = scale,
				Translation = bias,
				Vertices = new List<short>(padToCount * fields.Length)
			};

			foreach (float floatValue in vertices
				.SelectMany(v => fields
					.Select(f => (float)f.GetValue(v))
				)
			)
			{
				compressedMeshTransform.Vertices.Add(
					(short)Math.Round(
						((double)floatValue - bias) / scale
					)
				);
			}

			// Add extra zeroes, in case a fixed length of vertices is required. Animation sequences require each frame to have the same number of vertices. Note however, that adding zeroes will break the mesh animation.
			// This is an edge case that's simply as a fix to prevent crashes, but is not an actual solution and should typically be handled externally.
			for (var i = 0; i < (padToCount - vertices.Count) * fields.Length; i++)
			{
				compressedMeshTransform.Vertices.Add(0);
			}

			return compressedMeshTransform;
		}

		/// <summary>
		/// Gets the best possible compression factors required (least amount of loss of precision) to align compressed vertices into Int16 values.
		/// </summary>
		/// <typeparam name="T">A class/structure of <see cref="float" /> fields, like <see cref="Vector2" /> or <see cref="Vector3" />.</typeparam>
		/// <param name="vertices">The vertices to determine compression factors for.</param>
		/// <param name="bias">The bias for each vector component.</param>
		/// <param name="scale">The scaling factor for each vector component.</param>
		private static void GetCompressionRange<T>(IEnumerable<T> vertices, out float bias, out float scale)
			where T : struct
		{
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);

			float fmax = 0, fmin = 0;

			// We need highest and lowest possible floats from all vertices.
			foreach (float floatValue in vertices
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