using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private SlicedFilledImage timerImage;
    
    private int lastMoneyAmount;
    private Tweener runningTween;
    private int previewScore;

    private void Start()
    {
        previewScore = 0;
        OnMoneyChanged(Managers.Game.MoneyAmount);
        OnScoreChanged(Managers.Game.Score);
        OnTimeChanged(Define.GameInitialTime);
        Managers.Game.onMoneyAmountUpdate += OnMoneyChanged;
        Managers.Game.onScoreUpdate += OnScoreChanged;
        Managers.Game.onTimeUpdate += OnTimeChanged;
    }

    private void OnDestroy()
    {
        Managers.Game.onMoneyAmountUpdate -= OnMoneyChanged;
        Managers.Game.onScoreUpdate -= OnScoreChanged;
        Managers.Game.onTimeUpdate -= OnTimeChanged;
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

    private void OnScoreChanged(int score)
    {
        DOVirtual.Int(previewScore, score, 0.3f, v => scoreText.text = v.ToString());
        previewScore = score;
    }

    private void OnTimeChanged(float remainTime)
    {
        timerImage.fillAmount = remainTime / Define.GameInitialTime;
    }
}
