using System;
using System.Globalization;
using System.IO;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	/// <summary>
	/// Serializes and deserializes date values.
	/// </summary>
	public class DateTimeValueSerializer : ControllerValueSerializer<DateTime>
	{
		private const string DateFormat = "yyyyMMdd";

		/// <inheritdoc />
		public override void Serialize(BinaryWriter writer, ControllerSerializationContext serializationContext, DateTime value)
		{
			string date = value.ToString("yyyyMMdd");
			writer.Write(int.Parse(date));
		}

		/// <inheritdoc />
		public override DateTime Deserialize(BinaryReader reader, ControllerSerializationContext serializationContext)
		{
			string sDate = reader.ReadInt32().ToString();
			if (DateTime.TryParseExact(sDate, DateFormat, null, DateTimeStyles.None, out DateTime date))
			{
				return date;
			}

			// If a parse error occurred, this most like is because the days of month are 31, for instance for april, or 30-31 for feb, or even 29 if not a leap year. Correct this by attempting to parse by lowering the days.
			int days = int.Parse(sDate.Substring(6, 2));
			sDate = sDate.Remove(6);

			// 3 extra attempts max.
			for (var i = 0; i < 3; i++)
			{
				days--;
				if (DateTime.TryParseExact(sDate + days, DateFormat, null, DateTimeStyles.None, out date))
				{
					return date;
				}
			}

			throw new FormatException($"The date {sDate} is in unexpected format.");
		}
	}
}
