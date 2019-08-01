using System;
using System.Collections.Generic;
using System.IO;
using skwas.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class StateMachineControllerSerializer : IControllerSerializer
	{
		private static class Magics
		{
			public const int Entry = 0x6cf46e74;
			public const int Condition = 0x596344fb;
			public const int Action = 0x7e03767b;
		}

		public void Deserialize(Stream stream, RawController controller)
		{
			StateMachineController smc = EnsureControllerType(controller);

			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				smc.Unknown0 = reader.ReadInt32();
				smc.GraphName = reader.ReadNullTerminatedString();
				smc.Unknown1 = reader.ReadInt32();
				int entryCount = reader.ReadInt32();
				smc.Unknown2 = reader.ReadInt32();
				smc.Unknown3 = reader.ReadInt32();

				smc.StateEntries = new List<StateMachineEntry>(entryCount);

				for (int i = 0; i < entryCount; i++)
				{
					if (reader.ReadInt32() != Magics.Entry)
					{
						throw new IOException("Unexpected data encountered.");
					}

					smc.StateEntries.Add(ReadEntry(stream, reader));
				}
			}
		}

		public void Serialize(Stream stream, RawController controller)
		{
			StateMachineController smc = EnsureControllerType(controller);

			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{
				writer.Write(smc.Unknown0);
				writer.WriteNullTerminatedString(smc.GraphName);
				writer.Write(smc.Unknown1);
				writer.Write(smc.StateEntries.Count);
				writer.Write(smc.Unknown2);
				writer.Write(smc.Unknown3);

				for (int entryIndex = 0; entryIndex < smc.StateEntries.Count; entryIndex++)
				{
					StateMachineEntry entry = smc.StateEntries[entryIndex];
					WriteEntry(writer, entryIndex, entry);
				}
			}
		}

		private static StateMachineEntry ReadEntry(Stream stream, BinaryReader reader)
		{
			var newEntry = new StateMachineEntry
			{
				Index = reader.ReadInt32(),
				Name = reader.ReadNullTerminatedString()
			};

			while (stream.Position < stream.Length)
			{
				if (reader.ReadInt32() != Magics.Condition)
				{
					stream.Position -= 4;
					break;
				}

				newEntry.Conditions.Add(ReadCondition(stream, reader));
			}

			return newEntry;
		}

		private static StateMachineCondition ReadCondition(Stream stream, BinaryReader reader)
		{
			var newCondition = new StateMachineCondition
			{
				ParentEntryIndex = reader.ReadInt32(),
				GotoEntry = reader.ReadInt32(),
				Type = reader.ReadInt32(),
				Expression = reader.ReadNullTerminatedString(),
				Value = reader.ReadNullTerminatedString()
			};

			while (stream.Position < stream.Length)
			{
				if (reader.ReadInt32() != Magics.Action)
				{
					stream.Position -= 4;
					break;
				}

				newCondition.Actions.Add(ReadAction(reader));
			}

			return newCondition;
		}

		private static StateMachineAction ReadAction(BinaryReader reader)
		{
			var newAction = new StateMachineAction
			{
				ParentEntryIndex = reader.ReadInt32(),
				ParentConditionIndex = reader.ReadInt32(),
				Name = reader.ReadNullTerminatedString(),
				Value = reader.ReadNullTerminatedString()
			};
			return newAction;
		}

		private static void WriteEntry(BinaryWriter writer, int entryIndex, StateMachineEntry entry)
		{
			writer.Write(Magics.Entry);
			writer.Write(entryIndex);
			writer.WriteNullTerminatedString(entry.Name);

			for (int conditionIndex = 0; conditionIndex < entry.Conditions.Count; conditionIndex++)
			{
				StateMachineCondition condition = entry.Conditions[conditionIndex];
				WriteCondition(writer, entryIndex, conditionIndex, condition);
			}
		}

		private static void WriteCondition(BinaryWriter writer, int entryIndex, int conditionIndex, StateMachineCondition condition)
		{
			writer.Write(Magics.Condition);
			writer.Write(entryIndex);
			writer.Write(condition.GotoEntry);
			writer.Write(condition.Type);
			writer.WriteNullTerminatedString(condition.Expression);
			writer.WriteNullTerminatedString(condition.Value);

			foreach (StateMachineAction action in condition.Actions)
			{
				WriteAction(writer, entryIndex, conditionIndex, action);
			}
		}

		private static void WriteAction(BinaryWriter writer, int entryIndex, int conditionIndex, StateMachineAction action)
		{
			writer.Write(Magics.Action);
			writer.Write(entryIndex);
			writer.Write(conditionIndex);
			writer.WriteNullTerminatedString(action.Name);
			writer.WriteNullTerminatedString(action.Value);
		}

		private static StateMachineController EnsureControllerType(RawController controller)
		{
			if (controller is StateMachineController stateMachineController)
			{
				return stateMachineController;
			}

			throw new InvalidOperationException($"Expected controller of type {nameof(StateMachineController)}.");
		}
	}
}