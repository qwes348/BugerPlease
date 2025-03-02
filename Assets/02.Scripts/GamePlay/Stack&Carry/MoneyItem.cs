using UnityEngine;

public class MoneyItem : StackableItem
{

    public override void OnGetItem()
    {
        Managers.Game.MoneyAmount += Define.OneMoneyBundleAmount;
    }
}
