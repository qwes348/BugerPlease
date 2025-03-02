using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ClerkCanvas : DynamicCanvas
{
    [SerializeField]
    private GameObject panelObject;
    [SerializeField]
    private ClerkUpgradeContent upgradeContentPrefab;
    [SerializeField]
    private List<ClerkUpgradeContent> upgradeContents;
    
    private List<ClerkUpgradeInfoData> upgradeInfoDatas;
    

    public override void ActiveCanvas(bool active)
    {
        base.ActiveCanvas(active);
        
        if(active)
            InitAndActive();
    }

    private async UniTaskVoid InitAndActive()
    {
        upgradeInfoDatas = new List<ClerkUpgradeInfoData>();
        upgradeInfoDatas.AddRange(await Managers.Resource.LoadAssetsByLabel<ClerkUpgradeInfoData>("clerkUpgradeInfoData"));
        
        for (int i = 0; i < upgradeInfoDatas.Count; i++)
        {
            if (upgradeContents.Count <= i)
            {
                upgradeContents.Add(Instantiate(upgradeContentPrefab, upgradeContents[0].transform.parent));
            }
            
            upgradeContents[i].Init(upgradeInfoDatas[i]);
        }
        
        panelObject.SetActive(true);
    }
}
