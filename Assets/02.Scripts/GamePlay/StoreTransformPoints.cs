using System;
using UnityEngine;

public class StoreTransformPoints : MonoBehaviour
{
    [SerializeField][Header("Blue")]
    private Transform waitingLinePoint;
    [SerializeField][Header("Yellow")]
    private Transform orderLinePoint;
    [SerializeField][Header("Magenta")]
    private Transform spawnPoint;
    [SerializeField][Header("Red")]
    private Transform exitPoint;

    public Transform WaitingLinePoint => waitingLinePoint;
    public Transform OrderLinePoint => orderLinePoint;
    public Transform SpawnPoint => spawnPoint;
    public Transform ExitPoint => exitPoint;
    
    private void OnDrawGizmosSelected()
    {
        if (waitingLinePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(waitingLinePoint.position, 0.2f);
        }

        if (orderLinePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(orderLinePoint.position, 0.2f);
        }

        if (spawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
        }

        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(exitPoint.position, 0.2f);
        }
    }
}
