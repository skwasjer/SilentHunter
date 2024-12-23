using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace SilentHunter.FileFormats.ChunkedFiles;

/// <summary>
/// Reads/writes binary files that are based on individual chunks (of blocks).
/// </summary>
/// <typeparam name="TChunk">The chunk type implementing <see cref="IChunk" />.</typeparam>
public abstract class ChunkFile<TChunk> : IChunkFile, IDisposable
    where TChunk : class, IChunk
{
    private ObservableCollection<TChunk> _chunks;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChunkFile{TChunk}" /> class.
    /// </summary>
    protected ChunkFile()
    {
        _chunks = new ObservableCollection<TChunk>();
        _chunks.CollectionChanged += _chunks_CollectionChanged;
    }

    /// <summary>
    /// </summary>
    ~ChunkFile()
    {
        Dispose(false);
    }

    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">true when disposing, false when finalizing.</param>
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

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets whether this instance is disposed or not.
    /// </summary>
    protected bool IsDisposed
    {
        get;
        private set;
    }

    IList IChunkFile.Chunks { get => Chunks; }

    /// <summary>
    /// Gets a collection of all chunks in the ChunkFile.
    /// </summary>
    public virtual Collection<TChunk> Chunks { get => _chunks; }

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

    /// <summary>
    /// Loads chunks from specified <paramref name="filename" />.
    /// </summary>
    /// <param name="filename">The filename to load chunks from.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this object is disposed.</exception>
    public virtual async Task LoadAsync(string filename)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        await LoadAsync(fs).ConfigureAwait(false);
    }

    /// <summary>
    /// Loads chunks from specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to load chunks from.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this object is disposed.</exception>
    public abstract Task LoadAsync(Stream stream);

    /// <summary>
    /// Saves chunks to specified <paramref name="filename" />.
    /// </summary>
    /// <param name="filename">The filename to save chunks to.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this object is disposed.</exception>
    public virtual async Task SaveAsync(string filename)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await SaveAsync(fs).ConfigureAwait(false);
    }

    /// <summary>
    /// Saves chunks to specified <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">The stream to save chunks to.</param>
    /// <exception cref="ObjectDisposedException">Thrown when this object is disposed.</exception>
    public abstract Task SaveAsync(Stream stream);
}
