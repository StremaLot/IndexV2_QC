using Index.Domain.Assets;
using Index.Domain.FileSystem;
using Index.Profiles.QuakeChampions.Assets;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.FileSystem.Files
{

  public class QCSceneResourceFileNode :
    QCFileSystemNode,
    IFileSystemAssetNode<QCSceneAsset, QCSceneAssetFactory>
  {

    public QCSceneResourceFileNode( 
      IFileSystemDevice device, 
      fioZIP_CACHE_FILE.ENTRY entry, 
      IFileSystemNode parent = null ) 
      : base( device, entry, parent )
    {
    }

  }
}
