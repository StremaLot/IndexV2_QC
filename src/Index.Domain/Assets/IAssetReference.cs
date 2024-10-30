namespace Index.Domain.Assets
{

  public interface IAssetReference : IEquatable<IAssetReference>
  {

    string AssetName { get; }
    Type AssetType { get; }
    Type AssetFactoryType { get; }
    byte Priority => Node.Priority;

    string EditorKey { get; }

    IFileSystemAssetNode Node { get; }

  }

}
