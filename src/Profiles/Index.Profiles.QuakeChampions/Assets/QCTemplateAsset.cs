using Index.Domain.Assets;
using Index.Domain.Assets.Meshes;
using Index.Domain.FileSystem;
using Index.Jobs;
using Index.Profiles.QuakeChampions.Jobs;
using LibSaber.QuakeChampions.Structures.Resources;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Assets
{

  public class QCTemplateAsset : MeshAsset
  {
    #region Properties

    public override string TypeName => "Model";

    #endregion

    #region Constructor

    public QCTemplateAsset( IAssetReference assetReference )
      : base( assetReference )
    {
    }

    #endregion
  }

  public class QCTemplateAssetFactory : AssetFactoryBase<QCTemplateAsset>
  {

    #region Constructor

    public QCTemplateAssetFactory( IContainerProvider container )
      : base( container )
    {
    }

    #endregion

    #region Overrides

    public override IJob<QCTemplateAsset> LoadAsset( IAssetReference assetReference, IAssetLoadContext assetLoadContext = null )
    {
      if ( assetLoadContext is null )
        assetLoadContext = new AssetLoadContext();

      var parameters = new ParameterCollection();
      parameters.Set( assetReference );
      parameters.Set( assetLoadContext );

      return JobManager.StartJob<LoadTemplateJob>( parameters );
    }

    #endregion

  }

}
