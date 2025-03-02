using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerActionArea))]
public class PurchaseArea : MonoBehaviour
{
    [SerializeField]
    private Define.PurchasableType myPurchasableType;
    [SerializeField]
    private int price;
    [SerializeField]
    private PurchasableItem itemObject;

    private PlayerActionArea actionArea;

    private void Awake()
    {
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.PurchaseThresholdTime);
        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) => await Purchase());
    }

    public void Init(Define.PurchasableType purchasableType, int price, PurchasableItem itemObject)
    {
        myPurchasableType = purchasableType;
        this.price = price;
        this.itemObject = itemObject;
    }

    private async UniTask Purchase()
    {
        if(Managers.Game.MoneyAmount < price)
            return;
        Managers.Game.MoneyAmount -= price;
        
        StoreManager.Instance.MoneyEffect.StartEffect(StoreManager.Instance.Player.transform.position, transform.position);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        StoreManager.Instance.MoneyEffect.StopEffect();

        switch (myPurchasableType)
        {
            case Define.PurchasableType.TableSet:
                itemObject.OnPurchased();
                break;
        }
        
        gameObject.SetActive(false);
    }
}
