using Index.Domain.Assets;
using Index.Domain.FileSystem;
using Index.Profiles.QuakeChampions.Assets;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.FileSystem.Files
{

  public class QCTemplateResourceFileNode : 
    QCFileSystemNode, 
    IFileSystemAssetNode<QCTemplateAsset, QCTemplateAssetFactory>
  {

    #region Constructor

    public QCTemplateResourceFileNode( 
      IFileSystemDevice device, 
      fioZIP_CACHE_FILE.ENTRY entry, 
      IFileSystemNode parent = null )
      : base( device, entry, parent )
    {
    }

    #endregion

    #region Overrides

    public override bool IsHidden
    {
      get
      {
        //if ( ResourceDescription is null ) return true;
        //if ( string.IsNullOrEmpty( ResourceDescription.tpl ) ) return true;
        //if ( string.IsNullOrEmpty( ResourceDescription.tplData ) ) return true;
        //if ( DisplayName.StartsWith( "__x" ) ) return true;

        return false;
      }
    }

    #endregion

  }

}
