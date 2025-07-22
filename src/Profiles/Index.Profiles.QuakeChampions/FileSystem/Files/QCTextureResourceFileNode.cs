using Index.Domain.Assets;
using Index.Domain.Assets.Textures.Dxgi;
using Index.Domain.FileSystem;
using Index.Profiles.QuakeChampions.Assets;
using LibSaber.QuakeChampions.Files;
using LibSaber.QuakeChampions.Serialization;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.FileSystem.Files
{

  public class QCTextureResourceFileNode : QCFileSystemNode, IFileSystemAssetNode<DxgiTextureAsset, QCTextureAssetFactory>
  {

    #region Constructor

    public QCTextureResourceFileNode( IFileSystemDevice device, fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent = null )
      : base( device, entry, parent )
    {
    }

    #endregion

  }

}
