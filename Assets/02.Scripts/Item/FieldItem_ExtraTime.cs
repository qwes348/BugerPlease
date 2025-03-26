using UnityEngine;

public class FieldItem_ExtraTime : FieldItemBase
{
    protected override void TakeEffect()
    {
        Managers.Game.GameTime += ((FieldItemData_ExtraTime)myData).ExtraTime;
    }
}
