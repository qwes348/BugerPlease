using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : StaticMono<UpgradeManager>
{
    // 업그레이드 레벨 관리 딕셔너리들
    [SerializeField] [SerializedDictionary]
    private AYellowpaper.SerializedCollections.SerializedDictionary<Define.UpgradeType, int> upgradeDict;
    
    public Action<Define.UpgradeType> onClerkUpgrade;
    public Action<Define.UpgradeType> onPlayerUpgrade;

    public int GetCurrentUpgradeLevel(UpgradeInfoDataBase upgradeInfoData)
    {
        int result = 0;
        upgradeDict.TryGetValue(upgradeInfoData.UpgradeType, out result);
        return result;
    }

    /// <summary>
    /// 현재 업그레이드 레벨
    /// </summary>
    public int GetCurrentUpgradeLevel(Define.UpgradeType statType)
    {
        return upgradeDict.GetValueOrDefault(statType, 0);
    }

    public void Upgrade(UpgradeInfoDataBase upgradeInfoData)
    {
        if (upgradeDict.ContainsKey(upgradeInfoData.UpgradeType))
        {
            upgradeDict[upgradeInfoData.UpgradeType]++;
        }
        else
        {
            upgradeDict.Add(upgradeInfoData.UpgradeType, 1);
        }

        // 플레이어 타입은 11번부터
        if ((int)upgradeInfoData.UpgradeType > 10)
        {
            onPlayerUpgrade?.Invoke(upgradeInfoData.UpgradeType);
        }
        else
        {
            onClerkUpgrade?.Invoke(upgradeInfoData.UpgradeType);
        }
    }
}
