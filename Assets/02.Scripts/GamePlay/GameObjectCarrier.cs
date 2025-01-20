using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어&손님이 운반하는 짐을 관리하고, 연출을 계산하는 클래스
public class GameObjectCarrier : MonoBehaviour
{
    [SerializeField]
    private Transform carrierTransform;
    
    #region Private
    private List<Transform> carriedObjects = new List<Transform>();
    #endregion
    
    #region Properties

    public IReadOnlyList<Transform> CarriedObjects => carriedObjects;
    #endregion

    #region Action
    public Action<Transform> onCarriedAdded;
    public Action<int> onCarriedRemovedAt;
    #endregion    
    
    #region Const
    private const int BaseCarryableCount = 10;   // TODO: 업그레이드 구현한다면 이 값에 +해서 쓰기
    private const float SpaceY = 0.5f;
    private readonly Color gizmoColor = new Color(0f, 1f, 0f, 0.5f);
    #endregion
    
    public bool IsCanCarryMore => carriedObjects.Count < BaseCarryableCount;

    [Button]
    public void TestAddCarryingObject()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = Vector3.one * 0.2f;
        PushCarryingObject(obj.transform);
    }

    public void PushCarryingObject(Transform newCarried)
    {
        if (carriedObjects.Count >= BaseCarryableCount)
            return;
        
        newCarried.transform.SetParent(carrierTransform);
        newCarried.transform.localPosition = SpaceY * carriedObjects.Count * Vector3.up;
        carriedObjects.Add(newCarried);
        
        onCarriedAdded?.Invoke(newCarried);
    }

    public Transform PopCarryingObject()
    {
        if (carriedObjects.Count == 0)
            return null;

        var carried = carriedObjects[^1];
        carriedObjects.RemoveAt(carriedObjects.Count - 1);
        carried.transform.SetParent(null);
        onCarriedRemovedAt?.Invoke(carriedObjects.Count);
        return carried;
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
