using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;

public class CustomerLineController : MonoBehaviour
{
    [SerializeField]
    private Transform waitingLinePoint;
    [SerializeField]
    private Transform orderLinePoinnt;
    [SerializeField]
    private Transform spawnPoint;
    
    private Queue<CustomerController> waitingCustomers = new Queue<CustomerController>();
    private CustomerController currentOrderingCustomer;
    
    public Transform WaitingLinePoint => waitingLinePoint;
    public Transform OrderLinePoinnt => orderLinePoinnt;
    
    // 손님 프리팹 어드레서블
    private const string customerAddress = "Prefab/Customer";
    private const int customerAddressCount = 1;

    [Button]
    public async UniTask SpawnNewCustomer()
    {
        var newCustomer = (await Managers.Pool.PopAsync(customerAddress + Random.Range(1, customerAddressCount))).GetComponent<CustomerController>();
        newCustomer.transform.position = spawnPoint.position;
        newCustomer.Init(1);
        newCustomer.gameObject.SetActive(true);
        newCustomer.GoToPoint(waitingLinePoint.position + Vector3.forward * waitingCustomers.Count);
        waitingCustomers.Enqueue(newCustomer);
    }

    [Button]
    public async UniTask NextOrderRequest()
    {
        if(waitingCustomers.Count == 0)
            await SpawnNewCustomer();
        
        currentOrderingCustomer = waitingCustomers.Dequeue();
        currentOrderingCustomer.SetState(Define.CustomerState.Ordering);
        
        Debug.LogError("NextOrderRequest");
    }
    
    private void OnDrawGizmosSelected()
    {
        if (waitingLinePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(waitingLinePoint.position, 0.2f);
        }

        if (orderLinePoinnt != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(orderLinePoinnt.position, 0.2f);
        }

        if (spawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
        }
    }
}
