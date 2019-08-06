using System.Collections.Generic;
using System.ComponentModel;

namespace SilentHunter.Controllers
{
	/// <summary>
	/// Represents as the base for all mesh animation controllers.
	/// </summary>
	public abstract class MeshAnimationController : AnimationController
	{
		// NOTE: Only 'Frames' should be exposed to browsers.

		/// <summary>
		/// A list of key frames. Each frame describes which set of compressed vertices to use at which time. The 3D engine will have to perform vertex interpolation to smooth out the animation in between two frames.
		/// </summary>
		public List<AnimationKeyFrame> Frames;

		/// <summary>
		/// A list of compressed vertices. Each set of compressed vertices replaces the vertices of the source mesh for a given key frame and can used/referenced once or multiple times.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<CompressedVectors> CompressedFrames;

		/// <summary>
		/// Extra unknown data (only found on chunk related to head morphing I believe).
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public byte[] Unknown0;
	}
}