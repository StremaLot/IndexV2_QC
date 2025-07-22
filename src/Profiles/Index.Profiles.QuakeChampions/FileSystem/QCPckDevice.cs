using Index.Domain.FileSystem;
using Index.Profiles.QuakeChampions.FileSystem.Files;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.FileSystem;

public class QCPckDevice : FileSystemDeviceBase
{

  #region Data Members

  private readonly string _basePath;
  private readonly string _filePath;
  private readonly fioZIP_FILE _zipFile;
  private readonly byte _nodePriority;
  private IFileSystemNode _rootNode;

  #endregion

  #region Constructor

  public QCPckDevice( string basePath, string filePath )
  {
    _basePath = basePath;
    _filePath = filePath;
    _zipFile = fioZIP_FILE.Open( _filePath );

    _nodePriority = GetPriority();
  }

  #endregion

  #region Overrides

  public override Stream GetStream( IFileSystemNode node )
  {
    var smNode = node as QCFileSystemNode;
    ASSERT( smNode != null, "Node is not an QCFileSystemNode." );

    return _zipFile.GetFileStream( smNode.Entry );
  }

  protected override Task<IResult<IFileSystemNode>> OnInitializing( CancellationToken cancellationToken = default )
  {
    return Task.Run( () =>
    {
        _rootNode = InitNodes();
        return ( IResult<IFileSystemNode> ) Result.Successful( _rootNode );
    } );
  }

  protected override void OnDisposing()
  {
    _zipFile?.Dispose();
    base.OnDisposing();
  }

  #endregion

  #region Private Methods

  private IFileSystemNode InitNodes()
  {
    var fileName = _filePath.Replace( _basePath, "" );
    var rootNode = new QCFileSystemNode( this, fileName );

    foreach ( var entry in _zipFile.Entries.Values )
    {
      CreateNode( entry, rootNode );
    }

    return rootNode;
  }

  private void CreateNode( 
    fioZIP_CACHE_FILE.ENTRY entry, 
    IFileSystemNode parent)
  {
    QCFileSystemNode node = null;

    var ext = Path.GetExtension( entry.FileName );


    //if ( ext == ".resource" )
    node = CreateResourceFileNode( entry, parent );

    //else
    //node = new QCFileSystemNode( this, entry, parent );

    node.Priority = _nodePriority;

    if ( node != null )
      parent.AddChild( node );
  }

  private QCFileSystemNode CreateResourceFileNode( 
    fioZIP_CACHE_FILE.ENTRY entry,
    IFileSystemNode parent )
  {
    var fileName = entry.FileName; //.Replace( ".resource", "" );
    var resourceExt = Path.GetExtension( fileName );

    switch(resourceExt)
    {
      case ".pct":
        return new QCTextureResourceFileNode( this, entry, parent );
      case ".tpl":
        return new QCTemplateResourceFileNode( this, entry, parent );
      case ".сtd":
        return new QCTextureDefinitionResourceFileNode(this, entry, parent );
      case ".lg":
        return new QCSceneResourceFileNode( this, entry, parent );
      default:
        return new QCFileSystemNode( this, entry, parent );
    }
  }

  private byte GetPriority()
  {
    if ( _filePath.Contains( @"\ultra\" ) )
      return byte.MaxValue;

    return 1;
  }

  #endregion

}
