using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClerkUpgradeContent : MonoBehaviour
{
    [SerializeField]
    private ClerkUpgradeInfoData data;
    [SerializeField]
    private List<Image> levelImages = new List<Image>();
    [SerializeField]
    private TextMeshProUGUI upgradeText;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private Button upgradeButton;

    public void Init(ClerkUpgradeInfoData newData)
    {
        data = newData;
        switch (data.UpgradeType)
        {
            case Define.ClerkStatType.MoveSpeed:
                upgradeText.text = "이동 속도";
                break;
            case Define.ClerkStatType.CarryingCount:
                upgradeText.text = "운반 능력";
                break;
            case Define.ClerkStatType.HireClerk:
                upgradeText.text = "알바 고용";
                break;
        }

        int maxLevel = data.MaxLevel;
        for (int i = 1; i < maxLevel; i++)
        {
            var levelImageClone = Instantiate(levelImages[0], levelImages[0].transform.parent);
            levelImages.Add(levelImageClone);
        }

        priceText.text = string.Format(data.GetPrice(0).ToString(), "N0");
        upgradeButton.onClick.AddListener(Upgrade);
        
        MoneyCheck(Managers.Game.MoneyAmount);
        Managers.Game.onMoneyAmountUpdate += MoneyCheck;
    }

    /// <summary>
    /// 업그레이드 실행
    /// </summary>
    private void Upgrade()
    {
        int upgradeCost = data.GetPrice(ClerkManager.Instance.GetCurrentUpgradeLevel(data.UpgradeType) + 1);
        if (Managers.Game.MoneyAmount < upgradeCost)
        {
            MoneyCheck(Managers.Game.MoneyAmount);
            return;
        }
        
        Managers.Game.MoneyAmount -= upgradeCost;
        ClerkManager.Instance.LevelUpClerkStat(data.UpgradeType);
    }

    /// <summary>
    /// 현재 소유금액으로 업그레이드할 수 없으면 버튼 비활성화
    /// </summary>
    /// <param name="money"></param>
    private void MoneyCheck(int money)
    {
        // TODO: 현재레벨 변수 집어넣기
        upgradeButton.interactable = money >= data.GetPrice(ClerkManager.Instance.GetCurrentUpgradeLevel(data.UpgradeType) + 1);
    }

    private void OnDestroy()
    {
        Managers.Game.onMoneyAmountUpdate -= MoneyCheck;
    }
}
