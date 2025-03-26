using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FieldItemManager : StaticMono<FieldItemManager>
{
    [SerializeField]
    private List<FieldItemBase> activeItems = new List<FieldItemBase>();
    
    private List<FieldItemData> itemDatas = new List<FieldItemData>();
    private int lastPivotScore;

    private void Start()
    {
        lastPivotScore = 0;
        LoadDatas().Forget();
        Managers.Game.onScoreUpdate += OnScoreUpdated;
    }

    private async UniTaskVoid LoadDatas()
    {
        itemDatas = await Managers.Resource.LoadAssetsByLabel<FieldItemData>("fieldItemData");
    }

    private void OnScoreUpdated(int score)
    {
        if (score - lastPivotScore <= Define.FieldItemSpawnChanceScore)
        {
            return;
        }
        
        float randomValue = Random.value;

        float accumulatedValue = 0;
        foreach (Define.FieldItemType itemType in Enum.GetValues(typeof(Define.FieldItemType)))
        {
            accumulatedValue += Define.FieldItemSpawnChanceDict[itemType];
            if (randomValue < accumulatedValue)
            {
                Debug.Log($"당첨된 아이템 {itemType.ToString()}");
                if (itemType == Define.FieldItemType.None)
                    break;
                SpawnItem(itemType).Forget();
            }
        }

        lastPivotScore = score;
    }

    [Button]
    private void SpawnItemTest()
    {
        SpawnItem(Define.FieldItemType.ExtraTime).Forget();
    }
    
    private async UniTaskVoid SpawnItem(Define.FieldItemType itemType)
    {
        FieldItemData itemData = itemDatas.Find(i => i.ItemType == itemType);
        if (itemData == null)
            return;

        FieldItemBase item = (await Managers.Pool.PopAsync(itemData.PrefabReference)).GetComponent<FieldItemBase>();
        item.transform.position = GetRandomPosition();
        item.Init(itemData);
        item.gameObject.SetActive(true);
        activeItems.Add(item);
    }

    private Vector3 GetRandomPosition(int areaMask = NavMesh.AllAreas)
    {
        // NavMesh 데이터 가져오기
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
    
        // 랜덤 삼각형 선택
        int triangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
    
        // 삼각형의 정점 가져오기
        Vector3 point1 = navMeshData.vertices[navMeshData.indices[triangleIndex * 3]];
        Vector3 point2 = navMeshData.vertices[navMeshData.indices[triangleIndex * 3 + 1]];
        Vector3 point3 = navMeshData.vertices[navMeshData.indices[triangleIndex * 3 + 2]];
    
        // 삼각형 내부의 랜덤 포인트 계산
        float u = Random.value;
        float v = Random.value;
    
        if (u + v > 1)
        {
            u = 1 - u;
            v = 1 - v;
        }
    
        Vector3 randomPoint = point1 + u * (point2 - point1) + v * (point3 - point1);
    
        // 해당 포인트가 원하는 영역에 있는지 확인
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, areaMask))
        {
            return hit.position + (Vector3.up * 0.5f);
        }

        return randomPoint;
    }

    public void OnItemPushed(FieldItemBase item)
    {
        activeItems.Remove(item);
    }
}
