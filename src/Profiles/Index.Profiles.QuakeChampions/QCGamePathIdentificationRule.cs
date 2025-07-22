using Index.Domain.GameProfiles;

namespace Index.Profiles.QuakeChampions
{
  public class QCGamePathIdentificationRule : IGamePathIdentificationRule
  {

    public string BaseDirectoryName => "client";

    public IEnumerable<string> ChildPaths
    {
      get
      {
        yield return @"preload\paks\shared.pak";
      }
    }

  }
}
