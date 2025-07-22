using Index.Domain.Assets;
using Index.Domain.Assets.Meshes;
using Index.Jobs;
using Index.Profiles.QuakeChampions.Jobs;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Assets
{

  public class QCSceneAsset : MeshAsset
  {

    #region Properties

    public override string TypeName => "Map (Work in Progress)";

    #endregion

    #region Constructor

    public QCSceneAsset( IAssetReference assetReference )
      : base( assetReference )
    {
    }

    #endregion

  }

  public class QCSceneAssetFactory : AssetFactoryBase<QCSceneAsset>
  {

    #region Constructor

    public QCSceneAssetFactory( IContainerProvider container )
      : base( container )
    {
    }

    #endregion

    #region Overrides

    public override IJob<QCSceneAsset> LoadAsset( IAssetReference assetReference, IAssetLoadContext assetLoadContext = null )
    {
      var parameters = new ParameterCollection();
      parameters.Set( assetReference );
      parameters.Set( assetLoadContext );

      return JobManager.StartJob<LoadSceneJob>( parameters );
    }

    #endregion

  }

}
