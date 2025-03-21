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
    protected int upgradeLevel = 0;

    protected virtual void Start()
    {
        UpgradeManager.Instance.onPlayerUpgrade += OnUpgraded;
    }

    private void Update()
    {
        if (!platform.IsCanStackMore)
            return;
        
        timer += Time.deltaTime;
        if (timer >= BaseCoolTime - (0.35f * BaseCoolTime))
        {
            timer = 0f;
            CreateFood().Forget();
        }
    }

    protected virtual async UniTaskVoid CreateFood()
    {
        string foodId = "Prefab/" + foodType.ToString();
        Food food = (await Managers.Pool.PopAsync(foodId)).GetComponent<Food>();
        food.transform.position = transform.position;
        platform.Push(food);
    }

    protected virtual void OnUpgraded(Define.UpgradeType upgradeType)
    {
        if (upgradeType != Define.UpgradeType.P_FoodMakeSpeed)
            return;
        
        upgradeLevel = UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.P_FoodMakeSpeed);
    }
}
