using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.Game;
        Managers.Audio.PlayBgm(Define.Bgm.Game).Forget();
    }
    public override void Clear()
    {
        
    }
}
