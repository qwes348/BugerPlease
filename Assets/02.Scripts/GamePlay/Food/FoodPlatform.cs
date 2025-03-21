using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodPlatform : MonoBehaviour
{
    // 최대 음식저장용량 기본값
    private const int baseStackSize = 4;
    private ObjectStacker stacker;

    private int StackSize => baseStackSize + UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.P_FoodCounterCapacity);
    public bool IsCanStackMore => stacker.Count < StackSize;
    public int FoodCount => stacker.Count;

    private void Awake()
    {
        stacker = GetComponent<ObjectStacker>();
    }

    public bool Push(Food food)
    {
        if (!IsCanStackMore)
            return false;
        
        stacker.Push(food.GetComponent<Stackable>());
        food.gameObject.SetActive(true);
        return true;
    }

    public Food Pop()
    {
        return stacker.Count == 0 ? null : stacker.Pop().GetComponent<Food>();
    }
}
