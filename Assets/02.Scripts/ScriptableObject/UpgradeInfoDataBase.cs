using UnityEngine;

public class UpgradeInfoDataBase : ScriptableObject
{
    [SerializeField]
    private string uiTextName;
    [SerializeField]
    protected Define.UpgradeType upgradeType;
    [SerializeField][Label("최고 레벨")]
    protected int maxLevel;
    [SerializeField][Label("0레벨 가격")]
    protected int startPrice;
    [SerializeField][Label("레벨당 늘어나는 가격")]
    protected int additionalPricePerLevel;
    
    #region Propeprties

    public string UiTextName => uiTextName;
    public Define.UpgradeType UpgradeType => upgradeType;
    public int MaxLevel => maxLevel;
    #endregion    
    
    /// <summary>
    /// targetLevel로 업그레이드하기위한 금액을 구함
    /// </summary>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public int GetPrice(int targetLevel)
    {
        if (targetLevel == 0)
            return startPrice;
        return startPrice + additionalPricePerLevel * (targetLevel - 1);
    }
}
