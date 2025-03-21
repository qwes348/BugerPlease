using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어&손님이 짐을 운반하게 하고, 연출을 계산하는 클래스
public class ObjectCarrier : MonoBehaviour
{
    [SerializeField]
    private Transform carrierTransform;
    
    private List<Stackable> carriedObjects = new List<Stackable>();
    private int carryableCountAdder = 0;
    
    #region Properties

    public IReadOnlyList<Stackable> CarriedObjects => carriedObjects;
    public int Count => carriedObjects.Count;
    public int CarryableCountAdder { get; set; }
    #endregion

    #region Action
    public Action<Stackable> onCarriableAdded;
    public Action<int> onCarriableRemovedAt;
    #endregion    
    
    #region Const
    private const int BaseCarryableCount = 1;   // TODO: 업그레이드 구현한다면 이 값에 +해서 쓰기
    private const float SpaceY = 0.14f;
    private readonly Color gizmoColor = new Color(0f, 1f, 0f, 0.5f);
    #endregion
    
    public bool IsCanCarryMore => carriedObjects.Count < BaseCarryableCount + CarryableCountAdder;

    [Button]
    public void TestAddCarryingObject()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = Vector3.one * 0.2f;
        var carriable = obj.AddComponent<Stackable>();
        PushCarryingObject(carriable);
    }

    public void PushCarryingObject(Stackable newCarried)
    {
        if (!IsCanCarryMore)
            return;
        
        newCarried.transform.SetParent(carrierTransform);
        if(DOTween.IsTweening(newCarried.transform))
            DOTween.Kill(newCarried.transform);
        var pos = (SpaceY * carriedObjects.Count * Vector3.up) + carrierTransform.position;
        newCarried.transform.DOLocalJump(carrierTransform.InverseTransformPoint(pos), Define.StackJumpPower, 1, 0.3f);
        // newCarried.transform.position = (SpaceY * carriedObjects.Count * Vector3.up) + carrierTransform.position;
        carriedObjects.Add(newCarried);
        
        onCarriableAdded?.Invoke(newCarried);
    }

    public Stackable PopCarryingObject()
    {
        if (carriedObjects.Count == 0)
            return null;

        var carried = carriedObjects[^1];
        carriedObjects.RemoveAt(carriedObjects.Count - 1);
        carried.transform.SetParent(null);
        onCarriableRemovedAt?.Invoke(carriedObjects.Count);
        return carried;
    }

    public Define.StackableType GetNextCarriableType()
    {
        return carriedObjects.Count == 0 ? Define.StackableType.None : carriedObjects[^1].MyStackableType;
    }

    private void OnDrawGizmosSelected()
    {
        if (carrierTransform != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(carrierTransform.position, 0.1f);
        }
    }
}
