using Index.Domain.FileSystem;
using Index.Profiles.SpaceMarine2.FileSystem.Files;
using LibSaber.SpaceMarine2.Structures.Resources;

namespace Index.Profiles.SpaceMarine2.FileSystem;

public class SM2PckDevice : FileSystemDeviceBase
{

  #region Data Members

  private readonly string _basePath;
  private readonly string _filePath;
  private readonly fioZIP_FILE _zipFile;
  private readonly byte _nodePriority;

  #endregion

  #region Constructor

  public SM2PckDevice( string basePath, string filePath )
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
    var smNode = node as SM2FileSystemNode;
    ASSERT( smNode != null, "Node is not an SM2FileSystemNode." );

    return _zipFile.GetFileStream( smNode.Entry );
  }

  protected override Task<IResult<IFileSystemNode>> OnInitializing( CancellationToken cancellationToken = default )
  {
    return Task.Run( () =>
    {
      var rootNode = InitNodes();
      return ( IResult<IFileSystemNode> ) Result.Successful( rootNode );
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
    var rootNode = new SM2FileSystemNode( this, fileName );

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
    SM2FileSystemNode node = null;

    var ext = Path.GetExtension( entry.FileName );

    if ( ext == ".resource" )
      node = CreateResourceFileNode( entry, parent );
    else
      node = new SM2FileSystemNode( this, entry, parent );

    node.Priority = _nodePriority;

    if ( node != null )
      parent.AddChild( node );
  }

  private SM2FileSystemNode CreateResourceFileNode( 
    fioZIP_CACHE_FILE.ENTRY entry,
    IFileSystemNode parent )
  {
    var fileName = entry.FileName.Replace( ".resource", "" );
    var resourceExt = Path.GetExtension( fileName );

    switch(resourceExt)
    {
      case ".pct":
        return new SM2TextureResourceFileNode( this, entry, parent );
      case ".tpl":
        return new SM2TemplateResourceFileNode( this, entry, parent );
      case ".td":
        return new SM2TextureDefinitionResourceFileNode(this, entry, parent );
      case ".scn":
        return new SM2SceneResourceFileNode( this, entry, parent );
      default:
        return new SM2FileSystemNode( this, entry, parent );
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
