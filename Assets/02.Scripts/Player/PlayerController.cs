using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region const
    private const float BaseMoveSpeed = 1.5f;
    private const float BaseRotateSpeed = 1000f;
    #endregion
    
    #region AnimParam
    private readonly int animParamMoveSpeed = Animator.StringToHash("MoveSpeed");
    #endregion
    
    private Transform cameraTransform;
    private Animator anim;
    private Rigidbody rb;
    private ObjectCarrier foodCarrier;

    private float moveSpeedAdder = 0f;

    public Rigidbody Rb => rb;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        foodCarrier = GetComponent<ObjectCarrier>();
        foodCarrier.onCarriableAdded += stackable =>
        {
            Managers.Audio.PlaySfx(Define.Sfx.Carry).Forget();
        };
    }

    private void Start()
    {
        UpgradeManager.Instance.onPlayerUpgrade += OnPlayerUpgraded;
    }

    private void FixedUpdate()
    {
        Vector3 inputDir = GameUI.Instance.InputCanvas.GetInputDir();
        anim.SetFloat(animParamMoveSpeed, inputDir.magnitude);
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
        rb.MovePosition(rb.position + transform.forward * ((BaseMoveSpeed + moveSpeedAdder) * Time.fixedDeltaTime));
    }

    private void OnPlayerUpgraded(Define.UpgradeType upgradedType)
    {
        switch (upgradedType)
        {
            case Define.UpgradeType.P_MoveSpeed:
                moveSpeedAdder = UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.P_MoveSpeed) * 0.5f;
                break;
            case Define.UpgradeType.P_CarryingCount:
                foodCarrier.CarryableCountAdder = UpgradeManager.Instance.GetCurrentUpgradeLevel(Define.UpgradeType.P_CarryingCount);
                break;
        }
    }
}
