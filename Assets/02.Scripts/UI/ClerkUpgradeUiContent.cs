using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClerkUpgradeUiContent : UpgradeUiContentBase
{
    [SerializeField]
    private ClerkUpgradeInfoData data;

    public override void Init(UpgradeInfoDataBase newData)
    {
        base.Init(newData);
        data = (ClerkUpgradeInfoData)newData;
    }

    private void OnDestroy()
    {
        Managers.Game.onMoneyAmountUpdate -= MoneyCheck;
    }
}