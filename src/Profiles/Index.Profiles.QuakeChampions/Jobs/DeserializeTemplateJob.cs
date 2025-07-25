using System.Formats.Tar;
using Index.Domain.Assets;
using Index.Domain.Assets.Textures;
using Index.Domain.FileSystem;
using Index.Jobs;
using Index.Profiles.QuakeChampions.FileSystem.Files;
using Index.Profiles.QuakeChampions.Meshes;
using LibSaber.IO;
using LibSaber.QuakeChampions.Serialization;
using LibSaber.QuakeChampions.Structures;
using LibSaber.Serialization;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Jobs
{
  public class DeserializeTemplateJob : JobBase<SceneContext>
  {

    #region Properties

    private IFileSystem FileSystem { get; }

    #endregion

    #region Constructor

    public DeserializeTemplateJob( IContainerProvider container, IParameterCollection parameters )
      : base( container, parameters )
    {
      FileSystem = container.Resolve<IFileSystem>();
    }

    #endregion

    #region Overrides

    protected override async Task OnExecuting()
    {
      SetStatus( "Deserializing Template" );
      SetIndeterminate();

      var assetReference = Parameters.Get<IAssetReference>();
      var stream = assetReference.Node.Open();
      var reader = new NativeReader( stream, Endianness.LittleEndian );
      
      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var template = Serializer<animTPL>.Deserialize( reader, new SerializationContext() );

      Stream tplDataStream = null;
      var device = assetReference.Node.Device;
      // Сначала ищем файл с расширением .tpl_data
      var tplDataNode = device.EnumerateFiles()
        .FirstOrDefault( f => Path.GetFileNameWithoutExtension( f.Name ) == name && Path.GetExtension( f.Name ).Equals( ".tpl_data", StringComparison.OrdinalIgnoreCase ) );
      if ( tplDataNode != null )
      {
        tplDataStream = tplDataNode.Open();
      }
      else
      {
        // Если .tpl_data не найден, ищем .tpl с сигнатурой 1SERtpl
        var tplNodes = device.EnumerateFiles()
          .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name && Path.GetExtension( f.Name ).Equals( ".tpl", StringComparison.OrdinalIgnoreCase ) )
          .ToList();
        foreach ( var node in tplNodes )
        {
          using var candidateStream = node.Open();
          var buffer = new byte[7];
          candidateStream.Read( buffer, 0, 7 );
          var sig = System.Text.Encoding.ASCII.GetString( buffer );
          if ( sig == "1SERtpl" )
          {
            tplDataStream = node.Open();
            break;
          }
        }
      }
      if ( tplDataStream == null )
        tplDataStream = assetReference.Node.Open();

      var context = new SceneContext( name, template.GeometryGraph, tplDataStream );

      var textures = new Dictionary<string, ITextureAsset>();

      Parameters.Set( context );
      Parameters.Set( template );
      Parameters.Set( "Textures", textures );

      SetResult( context );
    }

    private class StubFileSystemNode : IFileSystemNode
        {
            public IFileSystemDevice Device => null;
            public IParameterCollection Metadata => null;
            public IFileSystemNode Parent => null;
            public IFileSystemNode FirstChild { get; set; }
            public IFileSystemNode NextSibling { get; set; }
            public string Name => string.Empty;
            public string DisplayName => string.Empty;
            public bool IsDirectory => false;
            public bool IsHidden => false;
            public byte Priority => 0;

            public void AddChild(IFileSystemNode node) { }
            public void AddSibling(IFileSystemNode node) { }
            public IEnumerable<IFileSystemNode> EnumerateChildren(bool recursive = false) => Enumerable.Empty<IFileSystemNode>();
            public IEnumerable<IFileSystemNode> EnumerateSiblings(bool includeThis = false) => Enumerable.Empty<IFileSystemNode>();
            public T GetMetadata<T>(string key) => default;
            public void SetMetadata<T>(string key, T value) { }
            public string GetPath(bool excludeRoot = true) => string.Empty;
            public Stream Open() => Stream.Null;
            public void Dispose() { }
        }





    #endregion

  }

}
