using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private SerializedDictionary<Define.Scene, List<GameObject>> objectsByScene;

    private bool ignoreClose;
    private bool hasChange;
    
    public void ActiveCanvas(bool active)
    {
        if (active)
        {
            if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
                Managers.Game.SetPause(true);
            Init();
            ActivateTween().Forget();
        }
        else
        {
            if (ignoreClose)
                return;
            if(hasChange)
                Managers.SaveLoad.Save();
            DeactivateTween().Forget();
        }
    }

    private void Init()
    {
        DOTween.defaultTimeScaleIndependent = true;
        bgmSlider.SetValueWithoutNotify(Managers.SaveLoad.localSaveData.BGMVolume);
        sfxSlider.SetValueWithoutNotify(Managers.SaveLoad.localSaveData.SFXVolume);
        bgmSlider.onValueChanged.AddListener(v => OnVolumeChanged(v, AudioDatabase.AudioCategory.BGM));
        sfxSlider.onValueChanged.AddListener(v => OnVolumeChanged(v, AudioDatabase.AudioCategory.SFX));

        Define.Scene currentSceneType = Managers.Scene.CurrentScene.SceneType;
        foreach (var pair in objectsByScene)
        {
            foreach (var go in pair.Value)
            {
                go.SetActive(currentSceneType == pair.Key);
            }
        }
    }

    private async UniTaskVoid ActivateTween()
    {
        ignoreClose = true;
        
        mainPanel.transform.localScale = Vector3.zero;
        await mainPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        ignoreClose = false;
    }

    private async UniTaskVoid DeactivateTween()
    {
        ignoreClose = true;

        await mainPanel.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
        if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
            Managers.Game.SetPause(false);
        Destroy(gameObject);
    }

    private void OnVolumeChanged(float value, AudioDatabase.AudioCategory category)
    {
        switch (category)
        {
            case AudioDatabase.AudioCategory.BGM:
                Managers.SaveLoad.localSaveData.BGMVolume = value;
                break;
            case AudioDatabase.AudioCategory.SFX:
                Managers.SaveLoad.localSaveData.SFXVolume = value;
                break;
        }
        
        hasChange = true;
    }

    public void DeleteHighScore()
    {
        Managers.SaveLoad.localSaveData.HighScore = 0;
        hasChange = true;
        MainMenuCanvas.Instance.UpdateHighScore();
    }

    public void RetryGame()
    {
        Time.timeScale = 1;
        Managers.Game.GameState = Define.GameState.GameOver;
        Managers.Scene.LoadSceneViaLoading(Define.Scene.Game);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        Managers.Game.GameState = Define.GameState.GameOver;
        Managers.Scene.LoadSceneViaLoading(Define.Scene.MainMenu);
    }
}
