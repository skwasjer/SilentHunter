using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using Moq;
using Xunit;

namespace skwas.IO.Tests
{
	public class ProgressStreamTests
	{
		private class ProgressReporter
			: IProgress<ProgressStream.Progress>
		{
			public int CallbackCount;
			public float LastPercentage;

			#region Implementation of IProgress<in Progress>

			/// <summary>
			/// Reports a progress update.
			/// </summary>
			/// <param name="value">The value of the updated progress.</param>
			public void Report(ProgressStream.Progress value)
			{
				CallbackCount++;
				LastPercentage = value.Percentage;
				Debug.WriteLine(LastPercentage);
			}

			#endregion
		}

		private static byte[] CreateStreamMock(out Mock<Stream> mockStream)
		{
			var buffer = new byte[4096];
			var streamSize = 100000000;
			var pos = 0;
			mockStream = new Mock<Stream>();
			mockStream.Setup(s => s.Length).Returns(() => streamSize).Verifiable();
			mockStream.Setup(s => s.Position).Returns(() => pos).Verifiable();
			mockStream
				.Setup(s => s.Read(buffer, 0, buffer.Length))
				.Callback(() =>
				{
					// Advance mocked stream forward.
					pos = Math.Min(pos + buffer.Length, streamSize);
				})
				.Returns(() => pos >= streamSize ? 0 : buffer.Length)
				.Verifiable();
			return buffer;
		}

		[Fact]
		public void reports_progress_via_action()
		{
			var callbackCount = 0;
			var lastPercentage = 0f;

			Action<ProgressStream.Progress> callback = progress =>
			{
				callbackCount++;
				lastPercentage = progress.Percentage;
				Debug.WriteLine(lastPercentage);
			};

			Mock<Stream> mockStream;
			var buffer = CreateStreamMock(out mockStream);

			using (var progressStream = new ProgressStream(mockStream.Object, callback))
			{
				callbackCount.Should().Be(0);
				lastPercentage.Should().Be(0);

				while (true)
				{
					var bytesRead = progressStream.Read(buffer, 0, buffer.Length);
					if (bytesRead <= 0) break;
				}
			}

			mockStream.Verify();

			callbackCount.Should().BeGreaterThan(0);
			lastPercentage.Should().Be(100f);
		}

		[Fact]
		public void reports_progress_via_progress_t()
		{
			var callback = new ProgressReporter();

			Mock<Stream> mockStream;
			var buffer = CreateStreamMock(out mockStream);

			using (var progressStream = new ProgressStream(mockStream.Object, callback))
			{
				callback.CallbackCount.Should().Be(0);
				callback.LastPercentage.Should().Be(0);

				while (true)
				{
					var bytesRead = progressStream.Read(buffer, 0, buffer.Length);
					if (bytesRead <= 0) break;
				}
			}

			mockStream.Verify();

			callback.CallbackCount.Should().BeGreaterThan(0);
			callback.LastPercentage.Should().Be(100f);
		}

		[Fact]
		public void expect_basestream_called()
		{
			var buf = new byte[1];

			var mockStream = new Mock<Stream>();

			using (var progressStream = new ProgressStream(mockStream.Object, delegate {  }))
			{
				progressStream.Flush();
				progressStream.Seek(0, SeekOrigin.Begin);
				progressStream.ReadByte();
				progressStream.WriteByte(0);

				progressStream.Position += 1;
				progressStream.SetLength(1);
			}

			mockStream.Verify(s => s.Flush(), Times.Once());
			mockStream.Verify(s => s.Seek(0, SeekOrigin.Begin), Times.Once());
			mockStream.Verify(s => s.Read(buf, 0, buf.Length), Times.Once());
			mockStream.Verify(s => s.Write(buf, 0, buf.Length), Times.Once());
			mockStream.Verify(s => s.Position, Times.AtLeast(2));
			mockStream.Verify(s => s.SetLength(1), Times.Once());
		}

		[Fact]
		public void when_initialized_with_invalid_arguments_throws()
		{
			Action<ProgressStream.Progress> callback = null;
			Progress<ProgressStream.Progress> progressT = null;
			var stream = new Mock<Stream>();

			Action action;

			action = () => new ProgressStream(null, callback);
			action.Should().Throw<ArgumentNullException>();

			action = () => new ProgressStream(stream.Object, callback);
			action.Should().Throw<ArgumentNullException>();

			action = () => new ProgressStream(stream.Object, progressT);
			action.Should().Throw<ArgumentNullException>();
		}
	}
}
