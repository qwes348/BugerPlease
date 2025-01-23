using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class ObjectStacker : MonoBehaviour
{
    [Header("Pool Id는 테스트용으로만 사용. 비워져있어도 문제없음")]
    public string poolId;
    public int row;
    public Vector3 leftTopCornerPos;
    public Vector2 space;

    public Action onPushed;
    public Action onEmpty;
    
    private Stack<Stackable> objectStack = new Stack<Stackable>();
    
    public int Count => objectStack.Count;

    [Button("현재 위치를 좌상단Pos로 지정하기")]
    public void SetLeftTopCornerFromCurrentPos()
    {
        leftTopCornerPos = transform.position;
    }

    public void Push(Stackable obj)
    {
        int rowIndex = objectStack.Count % row;
        int colIndex = objectStack.Count / row;

        Vector3 pos = Vector3.right * rowIndex * space.x;
        pos += Vector3.up * colIndex * space.y;
        pos += leftTopCornerPos;
        obj.transform.position = pos;
        obj.transform.SetParent(transform);
        objectStack.Push(obj);
        onPushed?.Invoke();
    }
    
    [Button("[TEST] 푸쉬")]
    public async void PushTest()
    {
        var obj = await Managers.Pool.PopAsync(poolId);
        obj.transform.SetParent(transform);
        obj.gameObject.SetActive(true);
        Push(obj.GetComponent<Stackable>());
    }

    [Button("[TEST] 팝")]
    public Stackable Pop()
    {
        if (objectStack.Count == 0)
        {
            return null;
        }

        var obj = objectStack.Pop();
        obj.transform.SetParent(null);
        if(objectStack.Count == 0)
            onEmpty?.Invoke();
        return obj;
    }

    public Stackable Peek()
    {
        if (objectStack.Count == 0)
            return null;
        
        return objectStack.Peek();
    }

    public void Clear()
    {
        while (objectStack.Count > 0)
        {
            var obj = objectStack.Pop();
            var poolable = obj.GetComponent<Poolable>();
            if(poolable != null)
                Managers.Pool.Push(poolable);
            else
                Destroy(obj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftTopCornerPos, 0.1f);
    }
}
