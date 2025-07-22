using Index.Domain.Assets;
using Index.Domain.FileSystem;
using Index.Profiles.QuakeChampions.FileSystem;
using Index.Profiles.QuakeChampions.FileSystem.Files;
using LibSaber.QuakeChampions.Structures.Resources;

namespace Index.Profiles.QuakeChampions.Assets
{

  public class QCTextureDefinitionAsset : QCTextAsset
  {

    public override string TypeName => "Material";

    public QCTextureDefinitionAsset( IAssetReference assetReference ) 
      : base( assetReference )
    {
    }

    internal override string GetAssetFilePath( IAssetReference assetReference )
    {
      var assetNode = assetReference.Node as QCFileSystemNode;//QCResourceFileNode<resDESC_TD>;
      ASSERT( assetNode is not null, "TextureDefinition asset file node is not a valid QCResourceFileNode" );

      var tdFileName = assetNode.Name; //assetNode.ResourceDescription.td;
      //ASSERT( !string.IsNullOrWhiteSpace( tdFileName ), "resDESC_TD header does not specify a TextureDefinition file." );

      return tdFileName;
    }

  }

}
