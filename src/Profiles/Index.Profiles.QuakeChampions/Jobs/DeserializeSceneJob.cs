using System.Xml.Linq;
using Index.Domain.Assets;
using Index.Domain.Assets.Textures;
using Index.Domain.FileSystem;
using Index.Jobs;
using Index.Profiles.QuakeChampions.FileSystem.Files;
using Index.Profiles.QuakeChampions.Meshes;
using LibSaber.IO;
using LibSaber.QuakeChampions.Serialization;
using LibSaber.QuakeChampions.Structures;
using Prism.Ioc;

namespace Index.Profiles.QuakeChampions.Jobs
{

  public class DeserializeSceneJob : JobBase<SceneContext>
  {

    #region Properties

    private IFileSystem FileSystem { get; }

    #endregion

    #region Constructor

    public DeserializeSceneJob( IContainerProvider container, IParameterCollection parameters )
      : base( container, parameters )
    {
      FileSystem = container.Resolve<IFileSystem>();
    }

    #endregion

    #region Overrides

    protected override async Task OnExecuting()
    {
      SetStatus( "Deserializing Scene" );
      SetIndeterminate();

      var assetReference = Parameters.Get<IAssetReference>();

      var cdListTask = LoadCdList();
      var classListTask = LoadClassList();

      var lgFile = GetLgFile( assetReference );
      //var lgDataFile = GetLgDataFile( assetReference );

      using var stream = lgFile.Open();
      var reader = new NativeReader(stream, Endianness.LittleEndian);

      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var scene = Serializer<scnSCENE>.Deserialize( reader );

      // NOTE: For some reason the lg_data files have a header, so the offsets are borked.
      // Use a StreamSegment to pretend the header isn't there
      var lgDataStream = lgFile.Open();
      //var lgDataViewStream = new StreamSegment( lgDataStream, 0x10, lgDataStream.Length - 0x10 );
      var context = new SceneContext( name, scene.GeometryGraph, lgDataStream );
      var textures = new Dictionary<string, ITextureAsset>();

      var cdList = await cdListTask;
      var classList = await classListTask;

      Parameters.Set( context );
      Parameters.Set( scene );
      Parameters.Set( "Textures", textures );
      Parameters.Set( cdList );
      Parameters.Set( classList );

      SetResult( context );
    }

        private class StubFileSystemNode : IFileSystemNode
        {
            public IFileSystemDevice Device => null;
            public IFileSystemNode Parent => null;
            public IFileSystemNode FirstChild { get; set; }
            public IFileSystemNode NextSibling { get; set; }
            public IParameterCollection Metadata { get; } = null;
            public string Name => string.Empty;
            public string DisplayName => string.Empty;
            public bool IsDirectory => false;
            public bool IsHidden => false;
            public byte Priority => 0;

            public void AddChild(IFileSystemNode node) { }
            public void AddSibling(IFileSystemNode node) { }
            public IEnumerable<IFileSystemNode> EnumerateChildren(bool recursive = false) => Enumerable.Empty<IFileSystemNode>();
            public IEnumerable<IFileSystemNode> EnumerateSiblings(bool includeThis = false) => Enumerable.Empty<IFileSystemNode>();
            public string GetPath(bool excludeRoot = true) => string.Empty;
            public Stream Open() => Stream.Null;
            public T GetMetadata<T>(string key) => default;
            public void SetMetadata<T>(string key, T value) { }
            public void Dispose() { }
        }

    private IFileSystemNode GetLgFile( IAssetReference assetReference )
    {
      var assetNode = assetReference.Node as QCSceneResourceFileNode;
      ASSERT( assetNode is not null, "Scene AssetNode is null." );
     
      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var device = assetReference.Node.Device;

      var lgFileNodes = device.EnumerateFiles()
        .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name )
        .Take( 10 )
        .ToList();
      return ReturnOnExtention( lgFileNodes, "lg" );
    }
    private IFileSystemNode GetLgDataFile( IAssetReference assetReference )
    {
      var assetNode = assetReference.Node as QCSceneResourceFileNode; //also need to change
      ASSERT( assetNode is not null, "Scene AssetNode is null." );

      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var device = assetReference.Node.Device;

      var lgDataFileNode = device.EnumerateFiles()
        .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name )
        .Take( 10 )
        .ToList();
      return ReturnOnExtention( lgDataFileNode, "light_probe2" );

    }
    private IFileSystemNode ReturnOnExtention( List<IFileSystemNode> FileNodes, string extention )
    {
      int number_file;
      number_file = 0;
      foreach ( var item in FileNodes )
      {
        var name = item.Name;
        
        name = name.Substring( name.Length - extention.Length );

        if ( name == extention )
        {
          return FileNodes[ number_file ];
        }
        number_file++;
      }

      return FileNodes[0];
    }

    private async Task<cdLIST> LoadCdList()
    {
      var assetReference = Parameters.Get<IAssetReference>();
      var assetNode = assetReference.Node as QCSceneResourceFileNode;
      

      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var device = assetReference.Node.Device;

      var CdFileNodes = device.EnumerateFiles()
        .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name )
        .Take( 10 )
        .ToList();
      var fsNode = ReturnOnExtention( CdFileNodes, "cd_list" );

      if ( fsNode is null )
        return null;

      var reader = new NativeReader( fsNode.Open(), Endianness.LittleEndian );
      var cdList = Serializer.Deserialize<cdLIST>( reader );
      return cdList;
    }

    private async Task<ClassList> LoadClassList()
    {
      var assetReference = Parameters.Get<IAssetReference>();
      var assetNode = assetReference.Node as QCSceneResourceFileNode;

      var name = Path.GetFileNameWithoutExtension( assetReference.AssetName );
      var device = assetReference.Node.Device;

      var CLFileNodes = device.EnumerateFiles()
        .Where( f => Path.GetFileNameWithoutExtension( f.Name ) == name )
        .Take( 10 )
        .ToList();
      var fsNode = ReturnOnExtention( CLFileNodes, "class_list" );

      if ( fsNode is null )
        return null;

      var reader = new NativeReader( fsNode.Open(), Endianness.LittleEndian );
      var classList = Serializer.Deserialize<ClassList>( reader );
      return classList;
    }

    #endregion


  }

}
