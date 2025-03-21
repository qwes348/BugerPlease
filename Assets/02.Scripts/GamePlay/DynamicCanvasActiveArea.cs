using System;
using UnityEngine;

/// <summary>
/// 플레이어가 접촉하면 비동기 캔버스를 불러오는 발판
/// </summary>
[RequireComponent(typeof(PlayerActionArea))]
public class DynamicCanvasActiveArea : MonoBehaviour
{
    [SerializeField]
    private Define.DynamicCanvasType canvasType = Define.DynamicCanvasType.None;
    [SerializeField]
    private bool destroyCanvasWhenOutArea = true;
    
    private PlayerActionArea actionArea;

    private void Awake()
    {
        actionArea = GetComponent<PlayerActionArea>();
    }

    private void Start()
    {
        actionArea.OnPlayerInArea += UniTaskHelper.Action(async (PlayerController pc) =>
        {
            if (GameUI.Instance.IsActiveDynamicCanvas(canvasType))
                return;
            var canvas = await GameUI.Instance.GetDynamicCanvas(canvasType);
            canvas.ActiveCanvas(true);
        });

        if(destroyCanvasWhenOutArea)
        {
            actionArea.OnPlayerOutArea += pc =>
            {
                if(GameUI.Instance.IsActiveDynamicCanvas(canvasType))
                    GameUI.Instance.SetActiveDynamicCanvas(canvasType, false);
            };
        }
    }
}
