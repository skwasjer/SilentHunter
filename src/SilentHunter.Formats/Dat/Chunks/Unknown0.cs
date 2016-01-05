namespace SilentHunter.Dat
{
#if DEBUG1
	public sealed class Unknown0 : DatChunk
	{
		public Unknown0()
			: base(Magics.Unknown0)
		{
		}

		public override bool SupportsId
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsParentId
		{
			get
			{
				return base.SupportsParentId;
			}
		}

		private List<MorphedVertex> _morphedVertices;

		protected override void OnDeserialize(Stream stream)
		{
			/*RegionStream RegionStream = stream as RegionStream;

			BinaryReader reader = new BinaryReader(stream, Encoding.ParseEncoding);

			Id = reader.ReadUInt64();

			_morphedVertices = new List<MorphedVertex>(reader.ReadInt32());

			UnknownData.Add(new UnknownChunkData(RegionStream == null ? stream.Position - 4 : RegionStream.BasePosition - 4, stream.Position - 4, _morphedVertices, "A list of morph info per vertex."));			

			for (int i = 0; i < _morphedVertices.Capacity; i++)
			{
				_morphedVertices.Add((MorphedVertex)reader.ReadStruct(typeof(MorphedVertex)));
			}*/

			base.OnDeserialize(stream);
		}

		protected override void OnSerialize(Stream stream)
		{
			//BinaryWriter writer = new BinaryWriter(stream, Encoding.ParseEncoding);
			//writer.Write(Id);

			base.OnSerialize(stream);
		}

		private struct MorphedVertex
		{
			public int A;
			public float A1;
			public int B;
			public float B1;
			public int C;
			public float C1;
			public int D;
			public float D1;

			public override string ToString()
			{
				return string.Format("{0} {1} - {2} {3} - {4} {5} - {6} {7}", A, A1, B, B1, C, C1, D, D1);
			}
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
#endif
}
