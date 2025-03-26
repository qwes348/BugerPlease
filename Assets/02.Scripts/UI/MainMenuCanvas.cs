using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class MainMenuCanvas : StaticMono<MainMenuCanvas>
{
    [SerializeField]
    private GameObject tapDetector;
    [SerializeField]
    private GameObject tapText;
    [SerializeField]
    private GameObject menuParent;
    [SerializeField]
    private TMP_Text highScoreText;
    [SerializeField]
    private CanvasGroup highScoreCanvasGroup;
    [SerializeField]
    private CanvasGroup gameStartCanvasGroup;
    [SerializeField]
    private CanvasGroup exitGameCanvasGroup;

    private void Start()
    {
        tapDetector.SetActive(true);
        tapText.SetActive(true);
        menuParent.SetActive(false);
    }

    public void onTap()
    {
        tapDetector.SetActive(false);
        tapText.SetActive(false);
        highScoreText.text = Managers.SaveLoad.localSaveData.HighScore.ToString("N0");

        MenuAppearTween().Forget();
    }

    private async UniTaskVoid MenuAppearTween()
    {
        highScoreCanvasGroup.alpha = 0;
        gameStartCanvasGroup.alpha = 0;
        exitGameCanvasGroup.alpha = 0;
        highScoreCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.down * 100f, 0f);
        gameStartCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.down * 100f, 0f);
        exitGameCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.down * 100f, 0f);
        
        menuParent.SetActive(true);
        await UniTask.Yield();
        
        highScoreCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.up * 100f, 0.5f);
        highScoreCanvasGroup.DOFade(1f, 0.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        
        gameStartCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.up * 100f, 0.5f);
        gameStartCanvasGroup.DOFade(1f, 0.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        
        exitGameCanvasGroup.transform.DOBlendableLocalMoveBy(Vector3.up * 100f, 0.5f);
        exitGameCanvasGroup.DOFade(1f, 0.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
    }

    public void GameStart()
    {
        Managers.Scene.LoadSceneViaLoading(Define.Scene.Game);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void UpdateHighScore()
    {
        highScoreText.text = Managers.SaveLoad.localSaveData.HighScore.ToString("N0");
    }
}
