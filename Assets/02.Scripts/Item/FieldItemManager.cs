using System.Collections.Generic;
using UnityEngine;

public class FieldItemManager : StaticMono<FieldItemManager>
{
    [SerializeField]
    private List<FieldItemBase> activeItems = new List<FieldItemBase>();
}
