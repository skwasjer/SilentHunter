using System;
using System.Globalization;
using System.IO;

namespace SilentHunter.Dat.Controllers.Serialization
{
	public class DateTimeValueSerializer : ControllerValueSerializer<DateTime>
	{
		private const string DateFormat = "yyyyMMdd";

		public override void Serialize(BinaryWriter writer, ControllerSerializationContext context)
		{
			string date = ((DateTime)context.Value).ToString("yyyyMMdd");
			writer.Write(int.Parse(date));
		}

		public override object Deserialize(BinaryReader reader, ControllerDeserializationContext context)
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

			return null;
		}
	}
}
