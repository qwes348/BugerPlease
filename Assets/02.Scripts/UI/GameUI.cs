using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : StaticMono<GameUI>
{
    [SerializeField]
    private PlayerInputCanvas inputCanvas;
    [SerializeField]
    private SpeechBubbleCanvas speechBubbleCanvas;
    [SerializeField]
    private HudCanvas hudCanvas;

    // 비동기적으로 생성/파괴하는 캔버스를 보관할 딕셔너리
    private Dictionary<Define.DynamicCanvasType, DynamicCanvas> dynamicCanvases = new Dictionary<Define.DynamicCanvasType, DynamicCanvas>();
    
    public PlayerInputCanvas InputCanvas => inputCanvas;
    public SpeechBubbleCanvas SpeechBubbleCanvas => speechBubbleCanvas;
    public HudCanvas HudCanvas => hudCanvas;

    public async UniTask<DynamicCanvas> GetDynamicCanvas(Define.DynamicCanvasType canvasType)
    {
        if(dynamicCanvases.TryGetValue(canvasType, out DynamicCanvas canvas))
            return canvas;
        
        await ActiveDynamicCanvas(canvasType, true);
        return dynamicCanvases[canvasType];
    }
    
    public async UniTask ActiveDynamicCanvas(Define.DynamicCanvasType canvasType, bool active)
    {
        if (active)
        {
            string address = string.Empty;
            switch (canvasType)
            {
                case Define.DynamicCanvasType.ClerkManage:
                    address = "Prefab/ClerkCanvas";
                    break;
            }

            var canvas = await Managers.Resource.InstantiateAsset<DynamicCanvas>(address);
            dynamicCanvases.Add(canvasType, canvas);
        }
        else
        {
            Destroy(dynamicCanvases[canvasType].gameObject);
            dynamicCanvases.Remove(canvasType);
        }
    }
}
