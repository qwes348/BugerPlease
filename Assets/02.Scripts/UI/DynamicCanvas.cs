using Cysharp.Threading.Tasks;
using UnityEngine;

public class DynamicCanvas : MonoBehaviour
{
    [SerializeField]
    private Define.DynamicCanvasType canvasType = Define.DynamicCanvasType.None;

    public virtual void ActiveCanvas(bool active)
    {
        if(!active)
            DestroyCanvas();
    }
    
    protected virtual void DestroyCanvas()
    {
        GameUI.Instance.SetActiveDynamicCanvas(canvasType, false).Forget();
    }
}
