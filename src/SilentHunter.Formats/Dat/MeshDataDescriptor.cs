using System.Diagnostics.CodeAnalysis;

namespace SilentHunter.Dat
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	internal enum MeshDataDescriptor
	{
		/// <summary>
		/// Ambient occlusion map.
		/// </summary>
		TMAP,

		/// <summary>
		/// Vertex normals.
		/// </summary>
		NORM
	}
}