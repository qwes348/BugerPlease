using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "FieldItemData", menuName = "Scriptable Objects/FieldItemData")]
public class FieldItemData : ScriptableObject
{
    [SerializeField]
    protected Define.FieldItemType itemType;
    [SerializeField]
    protected AssetReferenceGameObject prefabReference;
    
    public Define.FieldItemType ItemType => itemType;
    public AssetReferenceGameObject PrefabReference => prefabReference;
}
