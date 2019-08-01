using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat
{
	public class StateMachineController : RawController, IRawSerializable
	{
		private static class Magics
		{
			public const int Entry = 0x6cf46e74;
			public const int Condition = 0x596344fb;
			public const int Action = 0x7e03767b;
		}

		// NOTE: some unknown fields, but they always seem to be the same. So mark them advanced, so they don't show up in simple editor views.

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown0 = 0x423B410F;

		/// <summary>
		/// The name that this StateMachineClass controller can be referenced by.
		/// </summary>
		public string GraphName;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown1 = 0x73A2500A;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown2 = 0x24CE7F70;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int Unknown3;

		/// <summary>
		/// A list of state entries that make up the state (behavior) of a character or object.
		/// </summary>
		public List<StateMachineEntry> StateEntries;

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				Unknown0 = reader.ReadInt32();
				GraphName = reader.ReadNullTerminatedString();
				Unknown1 = reader.ReadInt32();
				int entryCount = reader.ReadInt32();
				Unknown2 = reader.ReadInt32();
				Unknown3 = reader.ReadInt32();

				StateEntries = new List<StateMachineEntry>(entryCount);

				for (int i = 0; i < entryCount; i++)
				{
					if (reader.ReadInt32() != Magics.Entry)
					{
						throw new IOException("Unexpected data encountered.");
					}

					var newEntry = new StateMachineEntry();
					StateEntries.Add(newEntry);
					newEntry.Index = reader.ReadInt32(); // Increments from 0 up.
					newEntry.Name = reader.ReadNullTerminatedString();

					while (stream.Position < stream.Length)
					{
						if (reader.ReadInt32() != Magics.Condition)
						{
							stream.Position -= 4;
							break;
						}

						var newCondition = new StateMachineCondition();
						newEntry.Conditions.Add(newCondition);
						newCondition.ParentEntryIndex = reader.ReadInt32();
						newCondition.GotoEntry = reader.ReadInt32();

						newCondition.Type = reader.ReadInt32();
						newCondition.Expression = reader.ReadNullTerminatedString();
						newCondition.Value = reader.ReadNullTerminatedString();

						while (stream.Position < stream.Length)
						{
							if (reader.ReadInt32() != Magics.Action)
							{
								stream.Position -= 4;
								break;
							}

							var newAction = new StateMachineAction();
							newCondition.Actions.Add(newAction);
							newAction.ParentEntryIndex = reader.ReadInt32();
							newAction.ParentConditionIndex = reader.ReadInt32();
							newAction.Name = reader.ReadNullTerminatedString();
							newAction.Value = reader.ReadNullTerminatedString();
						}
					}
				}
			}
		}

		/// <summary>
		/// When implemented, serializes the controller to specified <paramref name="stream" />.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write(Unknown0);
				writer.WriteNullTerminatedString(GraphName);
				writer.Write(Unknown1);
				writer.Write(StateEntries.Count);
				writer.Write(Unknown2);
				writer.Write(Unknown3);

				for (int i = 0; i < StateEntries.Count; i++)
				{
					writer.Write(Magics.Entry);

					StateMachineEntry entry = StateEntries[i];
					writer.Write(i);
					writer.WriteNullTerminatedString(entry.Name);

					for (int j = 0; j < entry.Conditions.Count; j++)
					{
						writer.Write(Magics.Condition);

						StateMachineCondition condition = entry.Conditions[j];
						writer.Write(i);
						writer.Write(condition.GotoEntry);
						writer.Write(condition.Type);
						writer.WriteNullTerminatedString(condition.Expression);
						writer.WriteNullTerminatedString(condition.Value);

						foreach (StateMachineAction action in condition.Actions)
						{
							writer.Write(Magics.Action);
							writer.Write(i);
							writer.Write(j);
							writer.WriteNullTerminatedString(action.Name);
							writer.WriteNullTerminatedString(action.Value);
						}
					}
				}
			}
		}
	}
}