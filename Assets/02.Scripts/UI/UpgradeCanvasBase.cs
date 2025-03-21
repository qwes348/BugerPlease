using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCanvasBase : DynamicCanvas
{
    [SerializeField]
    private string dataLabel;
    [SerializeField]
    private GameObject panelObject;
    [SerializeField]
    private UpgradeUiContentBase upgradeUiContentPrefab;
    [SerializeField]
    private List<UpgradeUiContentBase> upgradeContents;
    
    private List<UpgradeInfoDataBase> upgradeInfoDatas;

    public override void ActiveCanvas(bool active)
    {
        base.ActiveCanvas(active);

        if (active)
        {
            InitAndActivateCanvas().Forget();
        }
    }

    private async UniTaskVoid InitAndActivateCanvas()
    {
        upgradeInfoDatas = new List<UpgradeInfoDataBase>();
        upgradeInfoDatas.AddRange(await Managers.Resource.LoadAssetsByLabel<UpgradeInfoDataBase>(dataLabel));

        for (int i = 0; i < upgradeInfoDatas.Count; i++)
        {
            if (upgradeContents.Count <= i)
            {
                upgradeContents.Add(Instantiate(upgradeUiContentPrefab, upgradeContents[0].transform.parent));
            }
            
            upgradeContents[i].Init(upgradeInfoDatas[i]);
        }
        
        panelObject.SetActive(true);
    }
}
