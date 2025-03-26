using System;
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
    /// <summary>
    /// 돈 같은 아이템 획득 발동 딜레이 시간
    /// </summary>
    public const float GetItemTresholdTime = 0.15f;
    /// <summary>
    /// 버거 1개당 먹는데 걸리는 시간
    /// </summary>
    public const float EatingTimePerBurger = 0.5f;
    public const float StackJumpPower = 2.5f;
    
    public const float GameInitialTime = 180f;
    public const float DefaultBGMVolume = 0.4f;
    public const float DefaultSfxVolume = 0.5f;
    
    public const int PriceTableSet = 300;
    public const int MaxWaitingCustomerCount = 5;
    public const int MaxCustomerWantCount = 3;
    public const int OneMoneyBundleAmount = 100;
    public const int StartingMoney = 300;
    public const int Score_CustomerStartEating = 1000;
    public const int FieldItemSpawnChanceScore = 10000; // 이 값만큼 점수가 누적될때마다 필드아이템 소환 확률을 계산함

    public static readonly float[] BGMPitch = new float[] { 1.0f, 1.1f, 1.2f };

    // 필드 아이템 소환 확률 딕셔너리: item들의 순서는 확률 높음 -> 확률 낮음 순서임이 보장되어야 함
    public static readonly Dictionary<FieldItemType, float> FieldItemSpawnChanceDict = new Dictionary<FieldItemType, float>()
    {
        { FieldItemType.None, 0.6f },
        { FieldItemType.ExtraTime, 0.4f }
    };
    #endregion

    #region enum
    public enum Scene
    {
        MainMenu,
        Game,
        Loading,
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
    [Flags]
    public enum StackableType
    {
        None = 0,
        Food = 1 << 0,
        Trash = 1 << 1,
        Money = 1 << 3,
        All = int.MaxValue
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
    public enum SpeechBubbleType
    {
        BurgerCount,
        NoSeat
    }
    public enum ClerkState
    {
        Idle,
        MoveToBurger,
        MoveToCounter,
        MoveToCleaning,
        MoveToDumpster
    }
    public enum UpgradeType
    {
        None = -1,
        MoveSpeed,
        CarryingCount,
        HireClerk,
        P_MoveSpeed = 11,
        P_CarryingCount,
        P_FoodMakeSpeed,
        P_FoodCounterCapacity
    }
    public enum DynamicCanvasType
    {
        None = -1,
        ClerkManage,
        PlayerUpgrade
    }
    public enum FieldItemType
    {
        None = -1,
        ExtraTime
    }
    #endregion
}
