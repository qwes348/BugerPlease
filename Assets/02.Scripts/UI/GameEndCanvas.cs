using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class GameEndCanvas : MonoBehaviour
{
    [SerializeField]
    private RectTransform notifyTextTransform;
    [SerializeField]
    private GameObject panelObject;
    [SerializeField]
    private RectTransform highScoreIconTransform;
    
    [Header("텍스트들")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI customerCountText;
    [SerializeField]
    private TextMeshProUGUI burgerCountText;
    [SerializeField]
    private TextMeshProUGUI salesText;

    public void SetActiveCanvas(bool active)
    {
        if(active)
            ActivateSequence().Forget();
        gameObject.SetActive(active);
    }
    
    [Button]
    public async UniTaskVoid TweenTest()
    {
        notifyTextTransform.localScale = Vector3.one;
        notifyTextTransform.rotation = Quaternion.Euler(0, 0, 0);
        notifyTextTransform.gameObject.SetActive(true);
        panelObject.SetActive(false);
        var panelRect = panelObject.GetComponent<RectTransform>();
        float panelOriginY = panelRect.anchoredPosition.y;
        panelRect.anchoredPosition = new Vector2(0, (Screen.height + panelRect.sizeDelta.y) * -1f);
        
        await notifyTextTransform.DOScale(new Vector3(0.7f, 1.5f, 1f), 0.1f);
        notifyTextTransform.DOShakeRotation(2f, Vector3.forward * 20f, 10, 45f, true, ShakeRandomnessMode.Harmonic);
        await notifyTextTransform.DOScale(1f, 2.5f).SetEase(Ease.InExpo);
        
        notifyTextTransform.gameObject.SetActive(false);
        panelObject.SetActive(true);
        panelRect.DOAnchorPosY(panelOriginY, 1f).SetEase(Ease.OutBack);
    }

    private async UniTaskVoid ActivateSequence()
    {
        notifyTextTransform.localScale = Vector3.one;
        notifyTextTransform.rotation = Quaternion.Euler(0, 0, 0);
        notifyTextTransform.gameObject.SetActive(true);
        panelObject.SetActive(false);
        highScoreIconTransform.gameObject.SetActive(false);
        var panelRect = panelObject.GetComponent<RectTransform>();
        float panelOriginY = panelRect.anchoredPosition.y;
        panelRect.anchoredPosition = new Vector2(0, (Screen.height + panelRect.sizeDelta.y) * -1f);
        
        await notifyTextTransform.DOScale(new Vector3(0.7f, 1.5f, 1f), 0.1f);
        notifyTextTransform.DOShakeRotation(2f, Vector3.forward * 20f, 10, 45f, true, ShakeRandomnessMode.Harmonic);
        await notifyTextTransform.DOScale(1f, 2.5f).SetEase(Ease.InExpo);
        
        notifyTextTransform.gameObject.SetActive(false);
        
        InitResult();
        panelObject.SetActive(true);
        panelRect.DOAnchorPosY(panelOriginY, 1f).SetEase(Ease.OutBack);
    }

    private void InitResult()
    {
        DOVirtual.Int(0, Managers.Game.Score, 1f, x => scoreText.text = x.ToString("N0")).OnComplete(() =>
        {
            if (Managers.Game.IsNewHighScore)
            {
                HighScoreTween();
            }
        });
        DOVirtual.Int(0, StoreManager.Instance.SoldCustomerCount, 1f, x => customerCountText.text = x.ToString("N0"));
        DOVirtual.Int(0, StoreManager.Instance.SoldBurgerCount, 1f, x => burgerCountText.text = x.ToString("N0"));
        DOVirtual.Int(0, StoreManager.Instance.TotalSales, 1f, x => salesText.text = x.ToString("N0"));
    }

    [Button]
    public void HighScoreTween()
    {
        highScoreIconTransform.gameObject.SetActive(true);
        highScoreIconTransform.transform.DORotate(Vector3.up * 720f, 1f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
    }

    public void Retry()
    {
        Managers.Scene.LoadSceneViaLoading(Define.Scene.Game);
    }

    public void ToMainMenu()
    {
        Managers.Scene.LoadSceneViaLoading(Define.Scene.MainMenu);
    }
}
