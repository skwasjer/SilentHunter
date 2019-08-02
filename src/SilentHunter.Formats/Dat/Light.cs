namespace SilentHunter.Dat
{
	/// <summary>
	/// Represents a light source.
	/// </summary>
	public sealed class Light
	{
		internal Light()
		{
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
			return Type == LightType.Omni
				? $"Light ({Type}), color={Color}, attenuation={Attenuation}, radius={Radius}"
				: $"Light ({Type}), color={Color}, intensity={Attenuation}";
		}
	}
}