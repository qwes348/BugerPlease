using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeechBubbleCanvas : MonoBehaviour
{
    [SerializeField]
    private RectTransform burgerBubbleTransform;
    [SerializeField]
    private TextMeshProUGUI burgerCountText;
    [SerializeField]
    private RectTransform noSeatBubbleTransform;
    [SerializeField]
    private Vector3 offset;

    private CustomerController currentCustomer;
    private Camera mainCamera;
    private RectTransform currentActiveBubbleTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        burgerBubbleTransform.gameObject.SetActive(false);
        noSeatBubbleTransform.gameObject.SetActive(false);
    }

    public void ActiveBubble(CustomerController customer, Define.SpeechBubbleType bubbleType)
    {
        currentCustomer = customer;
        switch (bubbleType)
        {
            case Define.SpeechBubbleType.BurgerCount:
                burgerCountText.text = $"X {currentCustomer.WantBurgerCount.ToString()}";
                currentActiveBubbleTransform = burgerBubbleTransform;
                break;
            case Define.SpeechBubbleType.NoSeat:
                currentActiveBubbleTransform = noSeatBubbleTransform;
                break;
        }
        currentActiveBubbleTransform.gameObject.SetActive(true);
    }

    public void Clear()
    {
        currentCustomer = null;
        currentActiveBubbleTransform.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (currentCustomer == null)
            return;
        
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(currentCustomer.transform.position);
        screenPoint += offset;
        currentActiveBubbleTransform.position = screenPoint;
    }
}
