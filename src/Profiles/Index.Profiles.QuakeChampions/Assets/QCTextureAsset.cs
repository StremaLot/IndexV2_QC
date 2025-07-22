using Index.Domain.Assets;
using Index.Domain.Assets.Textures.Dxgi;
using Index.Jobs;
using Index.Profiles.QuakeChampions.Jobs;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Assets
{

  public class QCTextureAssetFactory : AssetFactoryBase<DxgiTextureAsset>
  {
    #region Constructor

    public QCTextureAssetFactory( IContainerProvider containerProvider )
      : base( containerProvider )
    {
    }

    #endregion

    #region Overrides

    public override IJob<DxgiTextureAsset> LoadAsset( IAssetReference assetReference, IAssetLoadContext assetLoadContext = null )
    {
      var parameters = new ParameterCollection();
      parameters.Set( assetReference );
      parameters.Set( assetLoadContext );

      return JobManager.StartJob<LoadTextureJob>( parameters );
    }

    #endregion
  }

}
