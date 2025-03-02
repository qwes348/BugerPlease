using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class HudCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI moneyText;
    
    private int lastMoneyAmount;
    private Tweener runningTween;

    private void Start()
    {
        OnMoneyChanged(Managers.Game.MoneyAmount);
        Managers.Game.onMoneyAmountUpdate += OnMoneyChanged;
    }

    private void OnMoneyChanged(int moneyAmount)
    {
        runningTween?.Kill();
        runningTween = 
            DOVirtual.Int(lastMoneyAmount, moneyAmount, 0.3f, v => moneyText.text = v.ToString()).OnComplete(() =>
            {
                lastMoneyAmount = moneyAmount;
                runningTween = null;
            });
    }
}
