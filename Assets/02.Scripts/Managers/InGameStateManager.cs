using System;
using UnityEngine;

public class InGameStateManager : StaticMono<InGameStateManager>
{
    private void Awake()
    {
        Managers.Game.Init();
    }

    private void Start()
    {
        // TODO: UI연출 이후 게임시작
        GameStart();
    }

    private void Update()
    {
        if (Managers.Game.GameState != Define.GameState.Running)
            return;

        Managers.Game.GameTime -= Time.deltaTime;
        if (Managers.Game.GameTime < 0)
        {
            GameOver();
        }
    }

    public void GameStart()
    {
        Managers.Game.GameState = Define.GameState.Running;
    }

    private void GameOver()
    {
        Managers.Game.GameState = Define.GameState.GameOver;
    }
}
