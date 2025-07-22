using Index.Domain.FileSystem;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.FileSystem;

public class QCFileSystemNode : FileSystemNodeBase
{

  #region Properties

  internal fioZIP_CACHE_FILE.ENTRY Entry { get; set; }
  public long SizeInBytes { get; set; }

  #endregion

  #region Constructor

  public QCFileSystemNode( IFileSystemDevice device, string name, IFileSystemNode parent = null )
      : base( device, name, parent )
  {
  }

  public QCFileSystemNode( IFileSystemDevice device, fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent = null )
        : base( device, entry.FileName, parent )
  {
    Entry = entry;
    SizeInBytes = entry.Size;
  }

  #endregion

}
