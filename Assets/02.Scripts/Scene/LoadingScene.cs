using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private TMP_Text loadingText;

    protected override void Init()
    {
        SceneType = Define.Scene.Loading;
        
        loadingText.DoText("불러오는 중...", 1f).SetLoops(-1, LoopType.Yoyo);
        Managers.Resource.ReleaseAll();
        LoadNextScene().Forget();
    }
    
    public override void Clear()
    {
        loadingText.DOKill();
    }

    private async UniTaskVoid LoadNextScene()
    {
        var op = Managers.Scene.LoadSceneAsync(Managers.Scene.NextSceneType);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            timer += Time.deltaTime;
            // 최소 로딩시간 2초 보장
            if (op.progress >= 0.9f && timer >= 2f)
            {
                op.allowSceneActivation = true;
            }

            await UniTask.Yield();
        }
    }
}
