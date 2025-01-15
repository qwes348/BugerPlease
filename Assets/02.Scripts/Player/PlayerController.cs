using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region const
    private const float BaseMoveSpeed = 1.5f;
    private const float BaseRotateSpeed = 1000f;
    #endregion
    
    #region AnimParam
    private readonly int animParamMoveAmount = Animator.StringToHash("MoveAmount");
    #endregion
    
    private Transform cameraTransform;
    private Animator anim;
    private Rigidbody rb;
    
    private float moveSpeedMultiplier = 1f;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 inputDir = GameUI.Instance.InputCanvas.GetInputDir();
        anim.SetFloat(animParamMoveAmount, inputDir.magnitude);
        if (inputDir == Vector3.zero)
            return;
        
        // 카메라 기준으로 방향 설정
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // y축 필요없으니 제거
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        // 인풋 방향과 카메라 방향을 조합해서 이동 방향을 계산
        Vector3 moveDir = (forward * inputDir.y + right * inputDir.x).normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, BaseRotateSpeed * Time.deltaTime));
        // 자연스러운 이동을 위해 플레이어는 앞으로만 이동
        rb.MovePosition(rb.position + transform.forward * (BaseMoveSpeed * Time.fixedDeltaTime));
    }
}
