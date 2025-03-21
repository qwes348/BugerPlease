using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManager
{
    [SerializeField]
    private float gameTime;
    [SerializeField]
    private int score;
    [SerializeField]
    private int combo;
    [SerializeField]
    private int level;
    [SerializeField]
    private int moneyAmount;
    [SerializeField]
    private Define.GameState gameState;
    [SerializeField]
    private bool isNewHighScore;

    #region 이벤트
    public Action<float> onTimeUpdate;
    public Action<int> onScoreUpdate;
    public Action<int> onMoneyAmountUpdate;
    public Action<Define.GameState> onGameStateChanged;
    #endregion
    
    #region 프로퍼티
    public float GameTime { get => gameTime;
        set
        {
            gameTime = value; 
            onTimeUpdate?.Invoke(gameTime);
        } 
    }
    public int Score => score;
    public int MoneyAmount
    {
        get => moneyAmount;
        set
        {
            moneyAmount = value;
            onMoneyAmountUpdate?.Invoke(moneyAmount);
        }
    }
    public Define.GameState GameState { get => gameState;
        set
        {
            gameState = value;
            onGameStateChanged?.Invoke(value);
        }
    }
    public bool IsNewHighScore
    {
        get => isNewHighScore; 
        set => isNewHighScore = value;
    }
    #endregion
    
    public void Init()
    {
        gameTime = Define.GameInitialTime;
        combo = -1;
        level = 0;
        score = 0;
        moneyAmount = Define.StartingMoney;
        gameState = Define.GameState.None;
        isNewHighScore = false;
    }

    public void Clear()
    {
        onTimeUpdate = null;
        onScoreUpdate = null;
        onMoneyAmountUpdate = null;
        onGameStateChanged = null;
    }

    public void AddScore(int add)
    {
        score += add;
        onScoreUpdate?.Invoke(score);
    }
}
