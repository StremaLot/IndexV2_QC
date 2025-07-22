using Index.Domain.Assets;
using Index.Domain.Assets.Text;
using Index.Domain.FileSystem;
using Index.Domain.Jobs;
using Index.Jobs;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Assets
{

  public abstract class QCTextAsset : TextAsset
  {

    public QCTextAsset( IAssetReference assetReference ) 
      : base( assetReference )
    {
    }

    internal abstract string GetAssetFilePath( IAssetReference assetReference );

  }

  public class QCTextAssetFactory<TAsset> : AssetFactoryBase<TAsset>
    where TAsset : QCTextAsset
  {

    private IFileSystem _fileSystem;

    public QCTextAssetFactory( IContainerProvider container ) 
      : base( container )
    {
      _fileSystem = container.Resolve<IFileSystem>();
    }

    public override IJob<TAsset> LoadAsset( IAssetReference assetReference, IAssetLoadContext assetLoadContext = null )
    {
      var asset = CreateAsset( assetReference );

      var assetFilePath = asset.GetAssetFilePath( assetReference );
      var matches = _fileSystem.EnumerateFiles().Where( x => x.Name.EndsWith( assetFilePath ) ).ToArray();
      var assetFileNode = _fileSystem.EnumerateFiles().SingleOrDefault( x => Path.GetFileName(x.Name) == assetFilePath );
      ASSERT( assetFileNode is not null, "Text asset's data file was not found." );

      asset.TextStream = assetFileNode.Open();
      return CompletedJob.FromResult( asset );
    }

    private TAsset CreateAsset( IAssetReference assetReference )
      => ( TAsset ) Activator.CreateInstance( typeof( TAsset ), new object[] { assetReference } );

  }

}
