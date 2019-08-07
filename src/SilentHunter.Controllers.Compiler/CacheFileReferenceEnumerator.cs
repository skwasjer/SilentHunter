using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using SilentHunter.Controllers.Compiler.BuildCache;

namespace SilentHunter.Controllers.Compiler
{
	internal class CacheFileReferenceEnumerator : IEnumerable<CacheFileReference>
	{
		private readonly IFileSystem _fileSystem;
		private readonly string _sourceFileDir;

		public CacheFileReferenceEnumerator(string sourceFileDir)
			: this(new FileSystem(), sourceFileDir)
		{
		}

		internal CacheFileReferenceEnumerator(IFileSystem fileSystem, string sourceFileDir)
		{
			_fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			_sourceFileDir = sourceFileDir ?? throw new ArgumentNullException(nameof(sourceFileDir));
		}

		public IEnumerator<CacheFileReference> GetEnumerator()
		{
			string p = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), _sourceFileDir));
			return _fileSystem.DirectoryInfo.FromDirectoryName(p)
				.GetFiles("*.cs", SearchOption.AllDirectories)
				.Select(f =>
					new CacheFileReference
					{
						// Save relative name.
						Name = f.FullName.Substring(p.Length + 1),
						LastModified = f.LastWriteTimeUtc,
						Length = f.Length
					}
				)
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
