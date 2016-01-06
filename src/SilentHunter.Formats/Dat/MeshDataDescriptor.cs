//#define UPDATE_ALL_AND_COPY_TOMOD

using System.Diagnostics.CodeAnalysis;

namespace SilentHunter.Dat
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	enum MeshDataDescriptor
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
