using Index.Domain.Assets;
using Index.Domain.Assets.Textures;
using Index.Domain.Assets.Textures.Dxgi;
using Index.Domain.FileSystem;
using Index.Jobs;
using Index.Profiles.QuakeChampions.Assets;
using Index.Textures;
using LibSaber.IO;
using LibSaber.QuakeChampions.Enumerations;
using LibSaber.QuakeChampions.Serialization;
using LibSaber.QuakeChampions.Structures;
using LibSaber.QuakeChampions.Structures.Resources;
using Prism.Ioc;
using Serilog;

namespace Index.Profiles.QuakeChampions.Jobs
{

  public class LoadTextureJob : JobBase<DxgiTextureAsset>
  {

    #region Data Members

    private readonly IAssetManager _assetManager;
    private readonly IDxgiTextureService _dxgiTextureService;

    private IAssetReference _assetReference;
    private pctPICTURE _picture;

    #endregion

    #region Constructor

    public LoadTextureJob( IContainerProvider container, IParameterCollection parameters )
      : base( container, parameters )
    {
      _assetManager = container.Resolve<IAssetManager>();
      _dxgiTextureService = container.Resolve<IDxgiTextureService>();

      _assetReference = Parameters.Get<IAssetReference>();
      Name = $"Loading {_assetReference.AssetName}";
    }

    #endregion

    #region Overrides

    protected override Task OnInitializing()
    {
      return Task.Run( () =>
      {
        SetStatus( "Deserializing Texture" );
        SetIndeterminate();

        var assetReference = Parameters.Get<IAssetReference>();
        _picture = DeserializePicture( assetReference );
      } );
    }

    protected override async Task OnExecuting()
    {
      var picture = _picture;
      var textureInfo = CreateTextureInfo( picture );

      SetSubStatus( "Preparing DXGI Texture" );
      var dxgiImage = _dxgiTextureService.CreateDxgiImageFromRawTextureData( picture.Data, textureInfo );

      var textureType = GetTextureType( _assetReference );
      var asset = new DxgiTextureAsset( _assetReference, textureType, dxgiImage );

      var textureDefinition = await GetTextureDefinitionAsset();
      if ( textureDefinition is null )
      {
        Log.Warning( "Failed to find a texture definition for {textureName}.", asset.AssetName );
      }
      else
        asset.AdditionalData.Add( textureDefinition.AssetName, textureDefinition.TextStream );

      SetResult( asset );
    }

    #endregion

    #region Private Methods

    private pctPICTURE DeserializePicture( IAssetReference assetReference )
    {
      var stream = assetReference.Node.Open();
      var reader = new NativeReader( stream, Endianness.LittleEndian );

      return Serializer.Deserialize<pctPICTURE>( reader );
    }

    private DxgiTextureInfo CreateTextureInfo( pctPICTURE picture )
    {
      return new DxgiTextureInfo
      {
        Width = picture.Width,
        Height = picture.Height,
        Depth = picture.Depth,
        FaceCount = picture.Faces,
        MipCount = picture.MipMapCount,
        Format = GetDxgiFormat( picture.Format )
      };
    }

    private DxgiTextureFormat GetDxgiFormat( QCTextureFormat format )
    {
      switch ( format )
      {
        case QCTextureFormat.ARGB8888:
          return DxgiTextureFormat.DXGI_FORMAT_B8G8R8A8_UNORM;
        case QCTextureFormat.ARGB16161616S:
          return DxgiTextureFormat.DXGI_FORMAT_R16G16B16A16_SNORM;
        case QCTextureFormat.ARGB16161616U:
          return DxgiTextureFormat.DXGI_FORMAT_R16G16B16A16_UNORM;
        case QCTextureFormat.BC6U:
          return DxgiTextureFormat.DXGI_FORMAT_BC6H_UF16;
        case QCTextureFormat.BC7:
        case QCTextureFormat.BC7A:
          return DxgiTextureFormat.DXGI_FORMAT_BC7_UNORM;
        case QCTextureFormat.DXN:
          return DxgiTextureFormat.DXGI_FORMAT_BC5_UNORM;
        case QCTextureFormat.DXT5A:
          return DxgiTextureFormat.DXGI_FORMAT_BC4_UNORM;
        case QCTextureFormat.AXT1:
        case QCTextureFormat.OXT1:
          return DxgiTextureFormat.DXGI_FORMAT_BC1_UNORM;
        case QCTextureFormat.R8U:
          return DxgiTextureFormat.DXGI_FORMAT_R8_UNORM;
        case QCTextureFormat.R16:
          return DxgiTextureFormat.DXGI_FORMAT_R16_SNORM;
        case QCTextureFormat.R16G16:
          return DxgiTextureFormat.DXGI_FORMAT_R16G16_SINT;
        case QCTextureFormat.RGBA16161616F:
          return DxgiTextureFormat.DXGI_FORMAT_R16G16B16A16_FLOAT;
        case QCTextureFormat.XT5:
          return DxgiTextureFormat.DXGI_FORMAT_BC3_UNORM;
        case QCTextureFormat.XRGB8888:
          return DxgiTextureFormat.DXGI_FORMAT_B8G8R8X8_UNORM;

        default:
          throw new NotSupportedException( "Invalid QC Texture Type." );
      }
    }

    private static TextureType GetTextureType( IAssetReference assetReference )
    {
      var assetName = Path.GetFileNameWithoutExtension( assetReference.AssetName );

      var suffixIndex = assetName.LastIndexOf( '_' );
      if ( suffixIndex == -1 )
        return TextureType.Diffuse;

      var suffix = assetName.Substring( suffixIndex + 1 );
      switch ( suffix )
      {
        case "nm":
        case "det":
          return TextureType.Normals;
        case "em":
          return TextureType.Emission;
        case "spec":
          return TextureType.SpecularColor;
        case "cube":
          return TextureType.Cubemap;
        case "lmdifdir":
        case "sm":
          return TextureType.Lightmap;

        case "akill":
        case "br":
        case "hdetm":
        case "mpmask":
          return TextureType.ChannelPacked;

        default:
          return TextureType.Diffuse;
      }

    }

    private async Task<QCTextureDefinitionAsset> GetTextureDefinitionAsset()
    {
      var textureDefinitionName = Path.ChangeExtension( _assetReference.AssetName, ".td" );
      textureDefinitionName = Path.GetFileName( textureDefinitionName );

      _assetManager.TryGetAssetReference( typeof( QCTextureDefinitionAsset ),
        textureDefinitionName, out var textureDefinitionAssetReference );

      if ( textureDefinitionAssetReference is null )
        return null;

      var textureDefinition = await _assetManager.LoadAssetAsync<QCTextureDefinitionAsset>( textureDefinitionAssetReference );
      return textureDefinition;
    }

    #endregion

  }

}
