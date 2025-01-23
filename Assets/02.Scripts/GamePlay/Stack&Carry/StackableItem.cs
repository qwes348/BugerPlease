using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 손에 들순 없고 쌓여있다가 다가가면 획득하는 아이템
/// </summary>
public abstract class StackableItem : Stackable
{
    public abstract void OnGetItem();

    public virtual async UniTask JumpToPointAndClear(Vector3 point)
    {
        await transform.DOJump(point, 1.5f, 1, 0.3f);
        Managers.Pool.Push(GetComponent<Poolable>());
    }
}
