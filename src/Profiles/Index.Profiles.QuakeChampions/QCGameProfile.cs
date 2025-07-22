using Index.Domain.FileSystem;
using Index.Domain.GameProfiles;
using Index.Profiles.QuakeChampions.FileSystem;

namespace Index.Profiles.QuakeChampions
{
  public class QCGameProfile : GameProfileBase
  {
    #region Properties

    public override string GameId => "QuakeChampions";
    public override string GameName => "Quake Champions";
    public override string Author => "'nouCk yMa'";

    public override IFileSystemLoader FileSystemLoader => new QCFileSystemLoader();
    public override IGamePathIdentificationRule IdentificationRule => new QCGamePathIdentificationRule();

    #endregion
  }
}
