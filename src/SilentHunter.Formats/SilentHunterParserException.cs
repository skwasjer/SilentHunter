﻿using System;
using System.Runtime.Serialization;

namespace SilentHunter
{
	/// <summary>
	/// Represents an exception thrown during parsing.
	/// </summary>
	public class SilentHunterParserException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SilentHunterParserException"/> using specified <paramref name="message"/>.
		/// </summary>
		/// <param name="message">The error message.</param>
		public SilentHunterParserException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SilentHunterParserException"/> using specified <paramref name="message"/> and <paramref name="innerException"/>.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="innerException">The inner exception.</param>
		public SilentHunterParserException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SilentHunterParserException" /> class with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info">info</paramref> parameter is null.</exception>
		/// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0).</exception>
		protected SilentHunterParserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}