using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodPlatform : MonoBehaviour
{
    private const int baseStackSize = 4;
    private ObjectStacker stacker;

    private int StackSize => baseStackSize; // TODO: 업그레이드 구현후에 추각 공간 더해서 리턴
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
        
        stacker.Push(food.gameObject);
        food.gameObject.SetActive(true);
        return true;
    }

    public Food Pop()
    {
        return stacker.Count == 0 ? null : stacker.Pop().GetComponent<Food>();
    }
}
