using System;
using UnityEngine;

public class PlayerUpgradeUiContent : UpgradeUiContentBase
{
    [SerializeField]
    private PlayerUpgradeInfoData data;
    
    public override void Init(UpgradeInfoDataBase newData)
    {
        base.Init(newData);
        data = (PlayerUpgradeInfoData)newData;
    }
}
