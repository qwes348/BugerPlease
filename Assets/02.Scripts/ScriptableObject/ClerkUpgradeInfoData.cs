using NaughtyAttributes;
using UnityEngine;
using Label = NaughtyAttributes.LabelAttribute;

[CreateAssetMenu(fileName = "ClerkUpgradeData", menuName = "Scriptable Objects/ClerkUpgradeData")]
public class ClerkUpgradeInfoData : ScriptableObject
{
    [SerializeField]
    private Define.ClerkStatType upgradeType;
    [SerializeField][Label("최고 레벨")]
    private int maxLevel;
    [SerializeField][Label("0레벨 가격")]
    private int startPrice;
    [SerializeField][Label("레벨당 늘어나는 가격")]
    private int additionalPricePerLevel;
    
    #region Propeprties
    public Define.ClerkStatType UpgradeType => upgradeType;
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
