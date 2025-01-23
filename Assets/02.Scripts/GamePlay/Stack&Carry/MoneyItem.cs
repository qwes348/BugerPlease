using UnityEngine;

public class MoneyItem : StackableItem
{

    public override void OnGetItem()
    {
        StoreManager.Instance.MoneyAmount += Define.OneMoneyBundleAmount;
    }
}
