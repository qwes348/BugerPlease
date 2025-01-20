using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Define 
{
    #region constant
    
    /// <summary>
    /// 들고가기, 내려놓기의 발동 딜레이 시간 
    /// </summary>
    public const float PickupPutdownTresholdTime = 0.5f;
    /// <summary>
    /// 구매 발판의 발동 딜레이 시간
    /// </summary>
    public const float PurchaseThresholdTime = 0.25f;

    public const int PriceTableSet = 300;
    
    public const float GameInitialTime = 60f;
    public const float DefaultBGMVolume = 0.4f;
    public const float DefaultSfxVolume = 0.5f;

    public static readonly float[] BGMPitch = new float[] { 1.0f, 1.1f, 1.2f };
    
    #endregion

    #region enum
    public enum Scene
    {
        Title,
        Game,
        Loading,
        Score
    }
    public enum GameState
    {
        None = 0,
        Running = 1,
        GameOver = 2
    }
    public enum Sfx
    {
        Move,
        Click,
        GameStart,
        GameOver
    }
    public enum Bgm
    {
        Title,
        Game,
        Score
    }
    public enum FoodType
    {
        None = -1,
        Hamburger,
        Frenchfries
    }
    public enum PurchasableType
    {
        None = -1,
        TableSet
    }
    public enum SeatState
    {
        Empty,
        Using
    }
    public enum TableState
    {
        Empty,
        Using,
        Used
    }
    public enum CustomerState
    {
        Waiting,
        Ordering,
        SeatWaiting,
        Eating,
        Exiting
    }
    #endregion
}
