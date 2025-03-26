using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
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
    [SerializeField]
    private TMP_Text priceText;

    private PlayerActionArea actionArea;
    private bool isPurchased = false;
    
    public bool IsPurchased => isPurchased;

    private void Awake()
    {
        actionArea = GetComponent<PlayerActionArea>();
        actionArea.Init(Define.PurchaseThresholdTime);
        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) => await Purchase());
    }

    public void Init(Define.PurchasableType purchasableType, int price, PurchasableItem itemObject)
    {
        myPurchasableType = purchasableType;
        UpdatePrice(price);
        this.itemObject = itemObject;
    }

    public void UpdatePrice(int price)
    {
        if (this.price > 0)
        {
            // 가격 인상시에는 올라가는 연출을 보여줌
            DOVirtual.Int(this.price, price, 0.5f, p => priceText.text = $"${p}");
        }
        else
        {
            // 처음 초기화에는 ui 바로 업데이트
            priceText.text = $"${price}";
        }
        this.price = price;
    }

    private async UniTask Purchase()
    {
        if(Managers.Game.MoneyAmount < price)
            return;
        Managers.Game.MoneyAmount -= price;
        isPurchased = true;
        
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
