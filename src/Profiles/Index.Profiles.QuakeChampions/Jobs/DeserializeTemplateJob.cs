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

      Stream tplDataStream;
      var device = assetReference.Node.Device;
      int number_tpl_file;
      number_tpl_file = 0;
      var tplDataNodes = device.EnumerateFiles()
        .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name )
        .Take( 5 ) 
        .ToList();
      
      foreach ( var item in tplDataNodes )
      {
        var name_tpl = item.Name;
        number_tpl_file= number_tpl_file +1;
        name_tpl = name_tpl.Substring( name_tpl.Length - 8 );
        
        if (name_tpl == "tpl_data")
          {
          break;
        }
      }

      if ( number_tpl_file == 0 )
        tplDataStream = assetReference.Node.Open(); 
      else
        tplDataStream = tplDataNodes[number_tpl_file-1].Open();


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
