using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SettingButton : MonoBehaviour
{
    public AssetReferenceGameObject settingCanvasPrefabRef;

    public void OnClick()
    {
        OpenSettingCanvas().Forget();
    }

    private async UniTaskVoid OpenSettingCanvas()
    {
        var settingCanvas = await Managers.Resource.InstantiateAsset<SettingCanvas>(settingCanvasPrefabRef);
        settingCanvas.ActiveCanvas(true);
    }
}
