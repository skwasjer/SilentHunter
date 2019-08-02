using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace skwas.IO
{
	/// <summary>
	/// Represents a stream that can track progress of the stream position versus the length of the stream. A seperate background task is used to monitor the position of the stream.
	/// </summary>
	/// <remarks>
	/// It is recommended to use this class with an <see cref="IProgress{Progress}"/> as opposed to using it with an <see cref="Action{Progress}"/>. Using it via an action, this allows using inline lambda, however, it also adds an extra function call upon each progress report.
	/// 
	/// ProgressStream internally uses <see cref="Task"/> to track progress, and calls the Progress callback from the Task context. If you wish to report progress in UI controls, make sure to marshal back to the main UI thread via Control.Invoke().
	/// 
	/// NOTE: When using for save progress, the underlying stream must have an estimated length set via <see cref="SetLength"/>, as such that the position is moving towards the end. When done writing to the stream, truncate the stream to the current position. If not done like this, the stream length will always be the last position, and thus the progress report will always indicate 100%. If you are not able to estimate the stream size with an error factor of around 10-20%, then there is no point in using ProgressStream.
	/// 
	/// WARNING: Do NOT use when I/O performance requirements are high.
	/// </remarks>
	/// <example>
	/// 
	/// Loading:
	///		using (var progressStream = new ProgressStream(baseStream, progress => updateStatusBar)) {
	///			progressStream.Read(...);
	///		}
	/// 
	/// Saving:
	///		baseStream.SetLength(1000000);	// Set estimate.
	/// 
	///		using (var progressStream = new ProgressStream(baseStream, progress => updateStatusBar)) {
	///			progressStream.Write(...);
	///			progressStream.SetLength(progressStream.Position);	// Truncate if stream ends up smaller than estimation.
	///		}
	/// 
	/// </example>
	public sealed class ProgressStream
		: Stream, IProgress<ProgressStream.Progress>
	{
		/// <summary>
		/// Represents progress info for the <see cref="ProgressStream" /> class.
		/// </summary>
		public struct Progress
		{
			internal Progress(float percentage, long position, long length)
			{
				Percentage = percentage;
				Position = position;
				Length = length;
			}

			/// <summary>
			/// Gets the progress percentage.
			/// </summary>
			public float Percentage { get; }
			/// <summary>
			/// Gets the stream position.
			/// </summary>
			public long Position { get; }
			/// <summary>
			/// Gets the stream length.
			/// </summary>
			public long Length { get; }
		}

		private const int Delay = 10;

		/// <summary>
		/// The base stream.
		/// </summary>
		private readonly Stream _baseStream;

		/// <summary>
		/// The progress callback.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Progress> _onProgressCallback;

		/// <summary>
		/// The instance to call progress updates on.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IProgress<Progress> _progressInstance;

		/// <summary>
		/// The background task.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Task _monitor;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// Initializes a new instance of <see cref="ProgressStream" /> and reports progress via an <see cref="Action{Progress}" />.
		/// </summary>
		/// <param name="stream">The stream to monitor progress on.</param>
		/// <param name="progress">The progress callback which receives periodic updates on the stream position.</param>
		public ProgressStream(Stream stream, Action<Progress> progress)
			: this(stream)
		{
			_onProgressCallback = progress ?? throw new ArgumentNullException(nameof(progress));
			_progressInstance = this;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ProgressStream" /> and reports progress via <see cref="IProgress{Progress}" />.
		/// </summary>
		/// <param name="stream">The stream to monitor progress on.</param>
		/// <param name="progress">The instance that implements <see cref="IProgress{T}" /> which receives periodic updates on the stream position.</param>
		public ProgressStream(Stream stream, IProgress<Progress> progress)
			: this(stream)
		{
			_progressInstance = progress ?? throw new ArgumentNullException(nameof(progress));
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ProgressStream" />.
		/// </summary>
		/// <param name="stream">The stream to monitor progress on.</param>
		private ProgressStream(Stream stream)
		{
			_baseStream = stream ?? throw new ArgumentNullException(nameof(stream));

			// Spawn a task that monitors the stream every 10ms.
			_cancellationTokenSource = new CancellationTokenSource();
			_monitor = Task.Run(async () =>
				{
					while (!_cancellationTokenSource.IsCancellationRequested)
					{
						NotifyProgress();
						await Task.Delay(Delay);
					}

					// Perform a last notification.
					NotifyProgress();
				},
				_cancellationTokenSource.Token);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cancellationTokenSource")]
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				// Cancel and wait for the monitor to complete.
				_cancellationTokenSource.Cancel();
				_monitor.Wait();

				_monitor?.Dispose();
				_monitor = null;

				_cancellationTokenSource?.Dispose();
				_cancellationTokenSource = null;

				_baseStream?.Dispose();

				_onProgressCallback = null;
			}
		}

		/// <summary>
		/// Reports a progress update.
		/// </summary>
		/// <param name="value">The value of the updated progress.</param>
		void IProgress<Progress>.Report(Progress value)
		{
			// If the stream was created with an action instead of passing an IProgress<T>, call the action.
			_onProgressCallback?.Invoke(value);
		}

		/// <summary>
		/// Process the progress event.
		/// </summary>
		private void NotifyProgress()
		{
			long pos = Position;
			long length = Length;

			// _progressInstance can be null as the class is spooling up or disposing.
			_progressInstance?.Report(new Progress(
				length > 0 ? (float)((decimal)pos / length) * 100 : 0,
				pos,
				length
			));
		}

		/// <summary>
		/// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to
		/// the underlying device.
		/// </summary>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		public override void Flush()
		{
			_baseStream.Flush();
		}

		/// <summary>
		/// When overridden in a derived class, sets the position within the current stream.
		/// </summary>
		/// <returns>
		/// The new position within the current stream.
		/// </returns>
		/// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter. </param>
		/// <param name="origin">
		/// A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to
		/// obtain the new position.
		/// </param>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support seeking, such as if the stream is
		/// constructed from a pipe or console output.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _baseStream.Seek(offset, origin);
		}

		/// <summary>
		/// When overridden in a derived class, sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes. </param>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support both writing and seeking, such as if the
		/// stream is constructed from a pipe or console output.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		public override void SetLength(long value)
		{
			_baseStream.SetLength(value);
		}

		/// <summary>
		/// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within
		/// the stream by the number of bytes read.
		/// </summary>
		/// <returns>
		/// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes
		/// are not currently available, or zero (0) if the end of the stream has been reached.
		/// </returns>
		/// <param name="buffer">
		/// An array of bytes. When this method returns, the buffer contains the specified byte array with the
		/// values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by
		/// the bytes read from the current source.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read
		/// from the current stream.
		/// </param>
		/// <param name="count">The maximum number of bytes to be read from the current stream. </param>
		/// <exception cref="T:System.ArgumentException">
		/// The sum of <paramref name="offset" /> and <paramref name="count" /> is
		/// larger than the buffer length.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="offset" /> or <paramref name="count" /> is
		/// negative.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return _baseStream.Read(buffer, offset, count);
		}

		/// <summary>
		/// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position
		/// within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">
		/// An array of bytes. This method copies <paramref name="count" /> bytes from
		/// <paramref name="buffer" /> to the current stream.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
		/// current stream.
		/// </param>
		/// <param name="count">The number of bytes to be written to the current stream. </param>
		/// <exception cref="T:System.ArgumentException">
		/// The sum of <paramref name="offset" /> and <paramref name="count" /> is
		/// greater than the buffer length.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" />  is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="offset" /> or <paramref name="count" /> is
		/// negative.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// <see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was
		/// closed.
		/// </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			_baseStream.Write(buffer, offset, count);
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns>
		/// true if the stream supports reading; otherwise, false.
		/// </returns>
		public override bool CanRead => _baseStream.CanRead;

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns>
		/// true if the stream supports seeking; otherwise, false.
		/// </returns>
		public override bool CanSeek => _baseStream.CanSeek;

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns>
		/// true if the stream supports writing; otherwise, false.
		/// </returns>
		public override bool CanWrite => _baseStream.CanWrite;

		/// <summary>
		/// When overridden in a derived class, gets the length in bytes of the stream.
		/// </summary>
		/// <returns>
		/// A long value representing the length of the stream in bytes.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		public override long Length => _baseStream.Length;

		/// <summary>
		/// When overridden in a derived class, gets or sets the position within the current stream.
		/// </summary>
		/// <returns>
		/// The current position within the stream.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
		public override long Position
		{
			get => _baseStream.Position;
			set => _baseStream.Position = value;
		}
	}
}