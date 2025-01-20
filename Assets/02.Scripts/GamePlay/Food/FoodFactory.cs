using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class FoodFactory : MonoBehaviour
{
    [SerializeField]
    protected Define.FoodType foodType;
    [SerializeField]
    protected FoodPlatform platform;
    [SerializeField]
    protected float BaseCoolTime = 2f;
    
    protected float timer = 0f;

    private void Update()
    {
        if (!platform.IsCanStackMore)
            return;
        
        timer += Time.deltaTime;
        if (timer >= BaseCoolTime)
        {
            timer = 0f;
            CreateFood();
        }
    }

    protected virtual async UniTaskVoid CreateFood()
    {
        string foodId = "Prefab/" + foodType.ToString();
        Food food = (await Managers.Pool.PopAsync(foodId)).GetComponent<Food>();
        platform.Push(food);
    }
}
