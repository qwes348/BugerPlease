using UnityEngine;

public class ExtraTimeFieldItem : FieldItemBase
{
    protected override void TakeEffect()
    {
        Managers.Game.GameTime += Define.ExtraTimeItemValue;
    }
}
