using Index.Domain.FileSystem;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Structures.Resources;

namespace Index.Profiles.SpaceMarine2.FileSystem.Files
{

  public class SM2ResourceFileNode : SM2FileSystemNode
  {

    #region Data Members

    protected resDESC _resourceDescription;

    #endregion

    #region Properties

    public resDESC ResourceDescription
    {
      get
      {
        if (_resourceDescription is null)
          _resourceDescription = LoadResourceDesc();

        return _resourceDescription;
      }
      set => _resourceDescription = value;
    }

    #endregion

    #region Constructor

    public SM2ResourceFileNode( IFileSystemDevice device, fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent = null )
      : base( device, entry, parent )
    {
    }

    #endregion

    #region Private Methods

    private resDESC LoadResourceDesc()
    {
      var device = Device as SM2PckDevice;
      using var descStream = device.GetStream( this );
      var reader = new NativeReader( descStream, Endianness.LittleEndian );

      return Serializer<resDESC>.Deserialize( reader );
    }

    #endregion

  }

  public abstract class SM2ResourceFileNode<TResDesc> : SM2ResourceFileNode
    where TResDesc : resDESC
  {

    #region Properties

    public new TResDesc ResourceDescription
    {
      get => ( TResDesc ) base.ResourceDescription;
      set => base.ResourceDescription = value;
    }

    #endregion

    #region Constructor

    public SM2ResourceFileNode( IFileSystemDevice device, fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent = null ) 
      : base( device, entry, parent )
    {
    }

    #endregion

    #region Overrides

    public override string DisplayName
    {
      get
      {
        const int RESOURCE_EXT_LENGTH = 9;
        var displayName = base.DisplayName.Substring( 0, base.DisplayName.Length - RESOURCE_EXT_LENGTH );
        return Path.GetFileName( displayName );
      }
    }

    #endregion

  }

}
