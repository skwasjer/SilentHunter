using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using SilentHunter.FileFormats;

namespace SilentHunter.Testing.FluentAssertions
{
	public class ByteCollectionAssertions : GenericCollectionAssertions<byte>
	{
		public ByteCollectionAssertions(IEnumerable<byte> byteCollection)
			: base(byteCollection)
		{
		}

		protected override string Identifier => "byteCollection";

		private class CombinedState : IDisposable
		{
			public CompareState Expected { get; set; }
			public CompareState Subject { get; set; }

			public bool IsDone => Expected.IsDone && Subject.IsDone;
			public bool IsDoneSuccessfully => IsDone && Expected.Position == Subject.Position;

			public void Dispose()
			{
				Expected?.Dispose();
				Subject?.Dispose();
			}
		}

		private class CompareState : IDisposable
		{
			private IEnumerator<byte> _enumerator;

			public CompareState(IEnumerable<byte> obj)
			{
				Object = obj;
				_enumerator = obj.GetEnumerator();
			}

			public byte Current { get; private set; }
			public long Position { get; set; } = -1;
			private IEnumerable<byte> Object { get; }
			private Queue<byte> RingBuffer { get; } = new Queue<byte>();
			public bool IsDone { get; private set; }
			public string GetArea(int offset)
			{
				string hex = string.Join(" ", RingBuffer.Skip(offset).Select(b => b.ToString("x2")));
				string text = FileEncoding.Default.GetString(RingBuffer.Skip(offset).ToArray());

				return $"{hex}{Environment.NewLine}{Environment.NewLine}\t{text}";
			}

			public bool MoveNext()
			{
				bool r = _enumerator.MoveNext();
				if (r)
				{
					Current = _enumerator.Current;
					Position++;
					RingBuffer.Enqueue(_enumerator.Current);
					if (RingBuffer.Count > 32)
					{
						RingBuffer.Dequeue();
					}
				}
				else
				{
					IsDone = true;
				}
				return r;
			}

			public void Dispose()
			{
				_enumerator?.Dispose();
			}
		}

		public AndConstraint<ByteCollectionAssertions> BeEquivalentTo(IEnumerable<byte> expectedCollection, string because = null, params object[] reasonArgs)
		{
			if (expectedCollection == null)
			{
				throw new ArgumentNullException(nameof(expectedCollection));
			}

			CombinedState equivalency = Compare(Subject, expectedCollection);
			if (equivalency.IsDoneSuccessfully)
			{
				return new AndConstraint<ByteCollectionAssertions>(this);
			}

			Execute.Assertion
				.BecauseOf(because, reasonArgs)
				.Given(() => equivalency)
				.ForCondition(x => x.Expected.IsDone && !x.Subject.IsDone)
				.FailWith($"Expected byte {{1:x2}} at position {{0:X}} but found no more bytes{Environment.NewLine}Expected: {{2}}{Environment.NewLine}Subject: {{3}}",
					x => x.Expected.Position,
					x => x.Expected.Current,
					x => x.Expected.GetArea(0),
					x => x.Subject.GetArea(1)
				)
				.Then
				.ForCondition(x => !x.Expected.IsDone && x.Subject.IsDone)
				.FailWith($"Expected no more bytes at position {{0}} but found {{1:x2}}{Environment.NewLine}Expected: {{2}}{Environment.NewLine}Subject: {{3}}",
					x => x.Subject.Position,
					x => x.Subject.Current,
					x => x.Expected.GetArea(1),
					x => x.Subject.GetArea(0)
				)
				;

			return new AndConstraint<ByteCollectionAssertions>(this);
		}

		private static CombinedState Compare(IEnumerable<byte> subjectCollection, IEnumerable<byte> expectedCollection)
		{
			var subject = new CompareState(subjectCollection);
			var expected = new CompareState(expectedCollection);

			bool subjectHasMovedNext, expectedHasMovedNext;
			while (true)
			{
				subjectHasMovedNext = subject.MoveNext();
				expectedHasMovedNext = expected.MoveNext();
				if (!subjectHasMovedNext && !expectedHasMovedNext)
				{
					// Finished iteration.
					return new CombinedState { Subject = subject, Expected = expected };
				}

				if (
					subjectHasMovedNext != expectedHasMovedNext // One moved but the other did not.
				 || subject.Current != expected.Current // Bytes not equal
					)
				{
					return new CombinedState
					{
						Subject = subject,
						Expected = expected
					};
				}
			}
		}
	}
}
