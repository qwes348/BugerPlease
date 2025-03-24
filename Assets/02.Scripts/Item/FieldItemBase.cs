using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Poolable))]
public abstract class FieldItemBase : MonoBehaviour
{
    [SerializeField]
    private Define.FieldItemType itemType;
    
    private Collider myCollider;
    private Poolable myPoolable;

    public virtual void Init()
    {
        transform.localScale = Vector3.one;
        if(myCollider == null)
            myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = true;
        myCollider.isTrigger = true;
        
        myPoolable = GetComponent<Poolable>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        myCollider.enabled = false;
        transform.DOMove(other.transform.position, 0.25f);
        transform.DOScale(Vector3.one * 0.1f, 0.25f).OnComplete(() => Managers.Pool.Push(myPoolable));
        TakeEffect();
    }
    
    protected abstract void TakeEffect();
}
