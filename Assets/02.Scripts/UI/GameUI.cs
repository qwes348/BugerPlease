using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameUI : StaticMono<GameUI>
{
    [SerializeField]
    private PlayerInputCanvas inputCanvas;
    [SerializeField]
    private SpeechBubbleCanvas speechBubbleCanvas;
    [SerializeField]
    private HudCanvas hudCanvas;

    [Header("어드레서블 캔버스")]
    [SerializeField]
    private SerializedDictionary<Define.DynamicCanvasType, AssetReferenceGameObject> dynamicCanvasesDict;
    

    // 비동기적으로 생성/파괴하는 캔버스를 보관할 딕셔너리
    private Dictionary<Define.DynamicCanvasType, DynamicCanvas> activeDynamicCanvases = new Dictionary<Define.DynamicCanvasType, DynamicCanvas>();
    
    public PlayerInputCanvas InputCanvas => inputCanvas;
    public SpeechBubbleCanvas SpeechBubbleCanvas => speechBubbleCanvas;
    public HudCanvas HudCanvas => hudCanvas;

    public async UniTask<DynamicCanvas> GetDynamicCanvas(Define.DynamicCanvasType canvasType)
    {
        if(activeDynamicCanvases.TryGetValue(canvasType, out DynamicCanvas canvas))
            return canvas;
        
        await SetActiveDynamicCanvas(canvasType, true);
        return activeDynamicCanvases[canvasType];
    }
    
    public async UniTask SetActiveDynamicCanvas(Define.DynamicCanvasType canvasType, bool active)
    {
        dynamicCanvasesDict.TryGetValue(canvasType, out AssetReferenceGameObject addGo);
        if (addGo == null)
        {
            Debug.LogError($"다이나믹 캔버스 어드레서블 등록 안됨: {canvasType.ToString()}");
            return;
        }
        
        if (active)
        {
            var canvas = await Managers.Resource.InstantiateAsset<DynamicCanvas>(addGo);
            activeDynamicCanvases.Add(canvasType, canvas);
        }
        else
        {
            Destroy(activeDynamicCanvases[canvasType].gameObject);
            activeDynamicCanvases.Remove(canvasType);
        }
    }

    public bool IsActiveDynamicCanvas(Define.DynamicCanvasType canvasType)
    {
        return activeDynamicCanvases.ContainsKey(canvasType) &&
               activeDynamicCanvases[canvasType] != null &&
               activeDynamicCanvases[canvasType].gameObject.activeSelf;
    }
}
