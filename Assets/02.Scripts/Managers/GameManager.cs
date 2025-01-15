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
    private Define.GameState gameState;
    [SerializeField]
    private bool isNewHighScore;

    #region 이벤트
    public Action<float> onTimeUpdate;
    public Action<int> onScoreUpdate;
    public Action<int> onComboUpdate;
    public Action<int> onLevelUpdate;
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
    public int Combo { get => combo;
        set
        {
            combo = value;
            onComboUpdate?.Invoke(combo);
        }
        
    }
    public int Level { get => level;
        set
        {
            level = value;
            onLevelUpdate?.Invoke(level);
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
        gameTime = Define.GAME_INITIAL_TIME;
        combo = -1;
        level = 0;
        score = 0;
        gameState = Define.GameState.None;
        isNewHighScore = false;
    }

    public void Clear()
    {
        onTimeUpdate = null;
        onScoreUpdate = null;
        onComboUpdate = null;
        onLevelUpdate = null;
        onGameStateChanged = null;
    }

    public void AddScore(int add)
    {
        score += add;
        onScoreUpdate?.Invoke(score);
    }
}
