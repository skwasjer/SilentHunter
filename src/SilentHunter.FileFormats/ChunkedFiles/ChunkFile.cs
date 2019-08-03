using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.ChunkedFiles
{
	public abstract class ChunkFile<TChunk> : IChunkFile
		where TChunk : class, IChunk
	{
		private ObservableCollection<TChunk> _chunks;

		protected ChunkFile()
		{
			_chunks = new ObservableCollection<TChunk>();
			_chunks.CollectionChanged += _chunks_CollectionChanged;
		}

		~ChunkFile()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				// Dispose managed.
				if (_chunks != null)
				{
					_chunks.CollectionChanged -= _chunks_CollectionChanged;
				}

				_chunks = null;
			}
			// Dispose unmanaged.

			IsDisposed = true;
		}

		/// <summary>
		/// Releases all resources associated with this object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected bool IsDisposed
		{
			get;
			private set;
		}

		IList IChunkFile.Chunks => Chunks;

		/// <summary>
		/// Gets a collection of all chunks in the ChunkFile.
		/// </summary>
		public virtual Collection<TChunk> Chunks => _chunks;

		private void _chunks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Reset:
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (IChunk c in e.OldItems)
					{
						c.ParentFile = null;
					}

					break;
				case NotifyCollectionChangedAction.Add:
					foreach (IChunk c in e.NewItems)
					{
						c.ParentFile = this;
					}

					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (IChunk c in e.OldItems)
					{
						c.ParentFile = null;
					}

					foreach (IChunk c in e.NewItems)
					{
						c.ParentFile = this;
					}

					break;
				case NotifyCollectionChangedAction.Move:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public virtual async Task LoadAsync(string filename)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
			{
				await LoadAsync(fs).ConfigureAwait(false);
			}
		}

		public abstract Task LoadAsync(Stream stream);

		public virtual async Task SaveAsync(string filename)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
			{
				await SaveAsync(fs).ConfigureAwait(false);
			}
		}

		public abstract Task SaveAsync(Stream stream);
	}
}