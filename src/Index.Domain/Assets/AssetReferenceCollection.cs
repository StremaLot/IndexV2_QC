using System.Collections;
using System.Runtime.Serialization;

namespace Index.Domain.Assets
{

  public class AssetReferenceCollection<TAsset> : IAssetReferenceCollection<TAsset>
    where TAsset : IAsset
  {

    #region Data Members

    private readonly Dictionary<string, IAssetReference> _assetReferences;

    #endregion

    #region Properties

    public string AssetTypeName { get; }

    public int Count => _assetReferences.Count;

    #endregion

    #region Constructor

    public AssetReferenceCollection()
    {
      _assetReferences = new Dictionary<string, IAssetReference>();
      AssetTypeName = GetAssetTypeName();
    }

    #endregion

    #region Public Methods

    public void AddReference( IAssetReference assetReference )
    {
      ASSERT_NOT_NULL( assetReference );
      ASSERT( assetReference.AssetType == typeof( TAsset ), "Improper asset reference type." );

      if(!_assetReferences.TryAdd(assetReference.AssetName, assetReference))
      {
        var existingReference = _assetReferences[assetReference.AssetName];
        if( assetReference.Priority > existingReference.Priority )
          _assetReferences[assetReference.AssetName] = assetReference;
      }
    }

    public bool TryGetReference( string assetName, out IAssetReference assetReference )
      => _assetReferences.TryGetValue( assetName, out assetReference );

    #endregion

    #region Private Methods

    private static string GetAssetTypeName()
    {
      var dummy = ( IAsset ) FormatterServices.GetUninitializedObject( typeof( TAsset ) );
      return dummy.TypeName;
    }

    #endregion

    #region IEnumerable Methods

    public IEnumerator<IAssetReference> GetEnumerator()
    {
      foreach ( var assetReference in _assetReferences.Values )
        yield return assetReference;
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    #endregion

  }

}
