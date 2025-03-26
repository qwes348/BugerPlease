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
    [SerializeField]
    private Gradient timerImageGradient;
    
    private int lastMoneyAmount;
    private Tweener runningTween;
    private int previewScore;

    private void Start()
    {
        previewScore = 0;
        OnMoneyChanged(Managers.Game.MoneyAmount);
        OnScoreChanged(Managers.Game.Score);
        Managers.Game.onMoneyAmountUpdate += OnMoneyChanged;
        Managers.Game.onScoreUpdate += OnScoreChanged;
    }

    private void Update()
    {
        if (Managers.Game.GameState != Define.GameState.Running)
            return;
        float remainTime = Managers.Game.GameTime;
        timerImage.fillAmount = Mathf.MoveTowards(timerImage.fillAmount,remainTime / Define.GameInitialTime, Time.deltaTime * 10f);
        timerImage.color = timerImageGradient.Evaluate(timerImage.fillAmount);
    }

    private void OnDestroy()
    {
        Managers.Game.onMoneyAmountUpdate -= OnMoneyChanged;
        Managers.Game.onScoreUpdate -= OnScoreChanged;
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
}
