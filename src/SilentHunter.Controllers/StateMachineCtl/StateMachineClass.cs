/* 
 * StateMachineCtl.act - StateMachineClass
 *
 * © 2007-2016 skwas. All rights reserved.
 * This code is provided as is. Change at your own risk.
 * --------------------------------------------------
 * 
 * S3D template for the StateMachineClass controller of Silent Hunter.
 * 
 * For documentation on templates, requirements and restrictions, please refer to the documentation.
 * 
*/

using System.Collections.Generic;
using System.IO;
using skwas.IO;
using SilentHunter;
using SilentHunter.Dat;

namespace StateMachineCtl
{
	/// <summary>
	/// StateMachineCtl controller. State machine manager for characters or objects.
	/// </summary>
	public class StateMachineClass : IRawController, IRawSerializable
	{
		// NOTE: a, b, c and d are unknown fields to me, but they always seem to be the same.

		private int a = 0x423B410F;
		/// <summary>
		/// The name that this StateMachineClass controller can be referenced by.
		/// </summary>
		public string GraphName;
		private int b = 0x73A2500A;
		private int c = 0x24CE7F70;
		private int d = 0x0;

		/// <summary>
		/// A list of state entries that make up the state (behavior) of a character or object.
		/// </summary>
		public List<StateMachineEntry> StateEntries;

		// Magics.
		private static class Magics
		{
			public const int Entry = 0x6cf46e74;
			public const int Condition = 0x596344fb;
			public const int Action = 0x7e03767b;
		}

		/// <summary>
		/// When implemented, deserializes the controller from specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ParseEncoding, true))
			{
				a = reader.ReadInt32();
				GraphName = reader.ReadNullTerminatedString();
				b = reader.ReadInt32();
				var entryCount = reader.ReadInt32();
				c = reader.ReadInt32();
				d = reader.ReadInt32();

				StateEntries = new List<StateMachineEntry>(entryCount);

				for (var i = 0; i < entryCount; i++)
				{
					if (reader.ReadInt32() != Magics.Entry)
						throw new IOException("Unexpected data encountered.");

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
		/// When implemented, serializes the controller to specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream.</param>
		void IRawSerializable.Serialize(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.ParseEncoding, true))
			{

				writer.Write(a);
				writer.WriteNullTerminatedString(GraphName);
				writer.Write(b);
				writer.Write(StateEntries.Count);
				writer.Write(c);
				writer.Write(d);

				for (var i = 0; i < StateEntries.Count; i++)
				{
					writer.Write(Magics.Entry);

					var entry = StateEntries[i];
					writer.Write(i);
					writer.WriteNullTerminatedString(entry.Name);

					for (var j = 0; j < entry.Conditions.Count; j++)
					{
						writer.Write(Magics.Condition);

						var condition = entry.Conditions[j];
						writer.Write(i);
						writer.Write(condition.GotoEntry);
						writer.Write(condition.Type);
						writer.WriteNullTerminatedString(condition.Expression);
						writer.WriteNullTerminatedString(condition.Value);

						foreach (var action in condition.Actions)
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