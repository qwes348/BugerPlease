using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCarrier))]
public class PlayerCarrierSpringEffect : MonoBehaviour
{
    [SerializeField]
    private float stifness = 5f;    // 탄성
    [SerializeField]
    private float damping = 2f;     // 감쇠
    [SerializeField]
    private float inertiaMultiplier = 0.5f; // 관성 조절
    
    private PlayerCarrier carrier;
    private List<Vector3> velocities = new List<Vector3>();
    private Vector3 playerPrevPos;

    private void Awake()
    {
        carrier = GetComponent<PlayerCarrier>();
        carrier.onCarriedAdded += OnCarriedAdded;
        carrier.onCarriedRemovedAt += OnCarriedRemovedAt;
    }

    private void OnCarriedAdded(Transform newCarried)
    {
        velocities.Add(Vector3.zero);
    }

    private void OnCarriedRemovedAt(int removeIndex)
    {
        velocities.RemoveAt(removeIndex);
    }

    private void FixedUpdate()
    {
        Vector3 playerMovementDelta = transform.position - playerPrevPos;

        for (int i = 1; i < carrier.CarriedObjects.Count; i++)
        {
            // 플레이어 이동량을 반영한 목표 위치
            Vector3 targetPos = carrier.CarriedObjects[i - 1].position - playerMovementDelta * inertiaMultiplier;
            targetPos.y = carrier.CarriedObjects[i].position.y; // y축은 연출 안할거임
            
            // 스프링 힘 계산
            Vector3 force = (targetPos - carrier.CarriedObjects[i].position) * stifness;
            velocities[i] += force * Time.fixedDeltaTime;
            velocities[i] *= (1 - damping * Time.fixedDeltaTime);

            carrier.CarriedObjects[i].position += velocities[i] * Time.fixedDeltaTime;
        }
        
        playerPrevPos = transform.position;
    }
}
