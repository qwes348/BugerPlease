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
        // TODO: 나중에 연출 효과구현 / 돈 차감 구현
        await UniTask.Yield();

        switch (myPurchasableType)
        {
            case Define.PurchasableType.TableSet:
                itemObject.OnPurchased();
                break;
        }
        
        gameObject.SetActive(false);
    }
}
