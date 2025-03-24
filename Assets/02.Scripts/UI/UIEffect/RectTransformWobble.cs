using UnityEngine;

public class RectTransformWobble : MonoBehaviour
{
    public float wobbleAmountX = 10f; // X축 움직임 크기
    public float wobbleAmountY = 10f; // Y축 움직임 크기
    public float wobbleSpeed = 5f;   // 움직임 속도
    public float rotationAmount = 5f; // 회전 효과 크기

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition; // 원래 위치 저장
        originalRotation = rectTransform.localRotation;    // 원래 회전 저장
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }
        // 시간에 따라 X, Y축 위치를 사인 함수로 조절
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmountX;
        float wobbleY = Mathf.Cos(Time.time * wobbleSpeed) * wobbleAmountY;

        // 위치 업데이트
        rectTransform.anchoredPosition = originalPosition + new Vector2(wobbleX, wobbleY);

        // 회전 효과 추가
        float rotationZ = Mathf.Sin(Time.time * wobbleSpeed) * rotationAmount;
        rectTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, rotationZ);
    }
}
