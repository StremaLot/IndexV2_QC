using Index.Domain.FileSystem;

namespace Index.Profiles.QuakeChampions.FileSystem
{

  public class QCFileSystemLoader : FileSystemLoaderBase
  {
    protected override async Task OnLoadDevices()
    {
      await LoadFilesWithExtension( ".pak", path => new QCPckDevice( BasePath, path ) );
      //await LoadFileWithName( "default_tpl_0.pak", path => new QCPckDevice(BasePath, path));
    }
  }

}
