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
        scoreText.text = Managers.Game.Score.ToString("N0");
        customerCountText.text = StoreManager.Instance.SoldCustomerCount.ToString("N0");
        burgerCountText.text = StoreManager.Instance.SoldBurgerCount.ToString("N0");
        salesText.text = StoreManager.Instance.TotalSales.ToString("N0");
    }
}
