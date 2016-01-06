using System.Drawing;

namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents a light source.
	/// </summary>
	public sealed class Light
	{
		internal Light() {
		}

		/// <summary>
		/// Always zero.
		/// </summary>
		public uint Reserved0 { get; set; }

		/// <summary>
		/// Gets or sets the light type.
		/// </summary>
		public LightType Type { get; set; }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Gets or sets the attenuation.
		/// </summary>
		public float Attenuation { get; set; }

		/// <summary>
		/// Gets or sets the radius of the light source (Omni lights only).
		/// </summary>
		public float Radius { get; set; }

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			if (Type == LightType.Omni)
				return string.Format("Light ({0}), color={1}, attenuation={2}, radius={3}", Type, Color, Attenuation, Radius);
			else
				return string.Format("Light ({0}), color={1}, intensity={2}", Type, Color, Attenuation);
		}
	}
}