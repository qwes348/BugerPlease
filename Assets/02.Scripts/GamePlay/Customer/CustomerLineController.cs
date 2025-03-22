using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Threading;
using Random = UnityEngine.Random;

public class CustomerLineController : MonoBehaviour
{
    private Queue<CustomerController> waitingCustomers = new Queue<CustomerController>();
    private CustomerController currentOrderingCustomer;
    
    // 손님 프리팹 어드레서블
    private const string customerAddress = "Prefab/Customer";
    private const int customerAddressCount = 3;
    private CancellationTokenSource spawnCts;   // TODO: 게임 끝나면 취소 요청

    public async UniTask ContinuosSpawnCustomers()
    {
        // 이중으로 생성 태스크가 돌지 않도록 방지
        if (spawnCts != null)
            return;
        spawnCts = new CancellationTokenSource();

        while (!spawnCts.IsCancellationRequested)
        {
            if (waitingCustomers.Count >= Define.MaxWaitingCustomerCount)
            {
                // 손님줄이 Max만큼 길다면 1초간 대기
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                continue;
            }

            await SpawnNewCustomer();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }

    [Button]
    public async UniTask SpawnNewCustomer()
    {
        var newCustomer = (await Managers.Pool.PopAsync(customerAddress + Random.Range(1, customerAddressCount + 1))).GetComponent<CustomerController>();
        newCustomer.transform.position = StoreManager.Instance.TransformPoints.SpawnPoint.position;
        newCustomer.Init(Random.Range(1, Define.MaxCustomerWantCount + 1)); 
        newCustomer.gameObject.SetActive(true);
        newCustomer.GoToPoint(StoreManager.Instance.TransformPoints.WaitingLinePoint.position + Vector3.forward * waitingCustomers.Count);
        waitingCustomers.Enqueue(newCustomer);
    }

    [Button]
    public async UniTask NextOrderRequest()
    {
        // if(waitingCustomers.Count == 0)
        //     await SpawnNewCustomer();
        while (waitingCustomers.Count <= 0)
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        currentOrderingCustomer = waitingCustomers.Dequeue();
        currentOrderingCustomer.SetState(Define.CustomerState.Ordering);

        int i = 0;
        foreach (var customer in waitingCustomers)
        {
            customer.GoToPoint(StoreManager.Instance.TransformPoints.WaitingLinePoint.position + Vector3.forward * i);
            i++;
        }
    }
}
