using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 운반하는 짐을 관리하고, 연출을 계산하는 클래스
public class PlayerCarrier : MonoBehaviour
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
    #endregion
    
    public bool IsCanCarryMore => carriedObjects.Count < BaseCarryableCount;

    [Button]
    public void TestAddCarryingObject()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = Vector3.one * 0.2f;
        AddCarryingObject(obj.transform);
    }

    public void AddCarryingObject(Transform newCarried)
    {
        if (carriedObjects.Count >= BaseCarryableCount)
            return;
        
        carriedObjects.Add(newCarried);
        newCarried.transform.SetParent(carrierTransform);
        newCarried.transform.localPosition = SpaceY * carriedObjects.Count * Vector3.up;
        
        onCarriedAdded?.Invoke(newCarried);
    }
}
