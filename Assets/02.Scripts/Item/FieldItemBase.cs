using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Poolable))]
public abstract class FieldItemBase : MonoBehaviour
{
    protected Collider myCollider;
    protected Poolable myPoolable;
    protected FieldItemData myData;
    protected float rotateSpeed = 90f;

    public virtual void Init(FieldItemData data)
    {
        myData = data;
        transform.localScale = Vector3.one;
        if(myCollider == null)
            myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = true;
        myCollider.isTrigger = true;
        
        myPoolable = GetComponent<Poolable>();
    }

    protected virtual void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        myCollider.enabled = false;
        TakeEffect();
        DisappearTween(other.transform).Forget();
    }
    
    protected abstract void TakeEffect();

    protected virtual async UniTask DisappearTween(Transform playerTransform)
    {
        transform.DOMove(playerTransform.position, 0.25f);
        await transform.DOScale(Vector3.one * 0.1f, 0.25f);
        Managers.Pool.Push(myPoolable);
        FieldItemManager.Instance.OnItemPushed(this);
    }
}
