using System;
using UnityEngine;

public class StackPickupArea : MonoBehaviour
{
    [SerializeField]
    private ObjectStacker stacker;

    private float thresholdTimer = 0f;

    private const float PickupThresholdTime = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        thresholdTimer += Time.deltaTime;
        if (stacker.Count <= 0)
            return;
        
        var carrier = other.GetComponent<PlayerCarrier>();
        if (!carrier.IsCanCarryMore)
            return;
        
        if (thresholdTimer >= PickupThresholdTime)
        {
            carrier.AddCarryingObject(stacker.Pop().transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        thresholdTimer = 0f;
    }
}
