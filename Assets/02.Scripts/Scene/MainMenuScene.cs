using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.MainMenu;
        Managers.Audio.PlayBgm(Define.Bgm.MainMenu).Forget();
    }
    public override void Clear()
    {
        
    }
}
