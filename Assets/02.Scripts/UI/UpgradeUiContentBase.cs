using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpgradeUiContentBase : MonoBehaviour
{
    [SerializeField]
    protected List<Image> levelImages = new List<Image>();
    [SerializeField]
    protected TextMeshProUGUI upgradeText;
    [SerializeField]
    protected TextMeshProUGUI priceText;
    [SerializeField]
    protected Button upgradeButton;

    protected string levelDeactivatedColor = "#8C8C8C";
    protected string levelActivatedColor = "#FFFF00";
    protected UpgradeInfoDataBase baseData;

    public bool IsMaxLevel
    {
        get
        {
            if (baseData == null)
                return false;
            return UpgradeManager.Instance.GetCurrentUpgradeLevel(baseData.UpgradeType) >= baseData.MaxLevel;
        }
    }

    public virtual void Init(UpgradeInfoDataBase newData)
    {
        baseData = newData;
        upgradeText.text = newData.UiTextName;

        UpdateLevelImages();
        UpdatePriceText();
        
        upgradeButton.onClick.AddListener(Upgrade);
        
        MoneyCheck(Managers.Game.MoneyAmount);
        Managers.Game.onMoneyAmountUpdate += MoneyCheck;
    }

    private void OnDisable()
    {
        Managers.Game.onMoneyAmountUpdate -= MoneyCheck;
    }

    private void OnDestroy()
    {
        Managers.Game.onMoneyAmountUpdate -= MoneyCheck;
    }

    protected virtual void UpdateLevelImages()
    {
        int maxLevel = baseData.MaxLevel;
        for (int i = 0; i < maxLevel; i++)
        {
            if (i >= levelImages.Count)
            {
                var levelImageClone = Instantiate(levelImages[0], levelImages[0].transform.parent);
                levelImages.Add(levelImageClone);
            }

            // 현재 업그레이드 레벨을 컬러로 표시
            Color levelColor;
            if (UpgradeManager.Instance.GetCurrentUpgradeLevel(baseData.UpgradeType) > i)
            {
                ColorUtility.TryParseHtmlString(levelActivatedColor, out levelColor);
            }
            else
            {
                ColorUtility.TryParseHtmlString(levelDeactivatedColor, out levelColor);
            }
            levelImages[i].color = levelColor;
        }
    }

    protected virtual void UpdatePriceText()
    {
        if (IsMaxLevel)
        {
            upgradeButton.interactable = !IsMaxLevel;
            priceText.text = "MAX";
            return;
        }
        
        int level = Mathf.Clamp(UpgradeManager.Instance.GetCurrentUpgradeLevel(baseData.UpgradeType) + 1, 0, baseData.MaxLevel);
        priceText.text = string.Format(baseData.GetPrice(level).ToString(), "N0");
    }

    /// <summary>
    /// 업그레이드 실행
    /// </summary>
    protected virtual void Upgrade()
    {
        if (IsMaxLevel)
            return;
        
        int upgradeCost = baseData.GetPrice(UpgradeManager.Instance.GetCurrentUpgradeLevel(baseData.UpgradeType) + 1);
        if (Managers.Game.MoneyAmount < upgradeCost)
        {
            MoneyCheck(Managers.Game.MoneyAmount);
            return;
        }
        
        Managers.Game.MoneyAmount -= upgradeCost;
        UpgradeManager.Instance.Upgrade(baseData);
        UpdatePriceText();
        UpdateLevelImages();
    }

    /// <summary>
    /// 현재 소유금액으로 업그레이드할 수 없으면 버튼 비활성화
    /// </summary>
    protected virtual void MoneyCheck(int money)
    {
        if (IsMaxLevel)
            return;
        upgradeButton.interactable = money >= baseData.GetPrice(UpgradeManager.Instance.GetCurrentUpgradeLevel(baseData.UpgradeType) + 1);
    }
}
