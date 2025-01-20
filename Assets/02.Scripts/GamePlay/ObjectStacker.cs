using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ObjectStacker : MonoBehaviour
{
    [NaughtyAttributes.Label("테스트용 풀ID(테스트에만 사용)")]
    public string poolId;
    public int row;
    public Vector3 leftTopCornerPos;
    public Vector2 space;
    
    private Stack<GameObject> objectStack = new Stack<GameObject>();
    
    public int Count => objectStack.Count;

    [Button("현재 위치를 좌상단Pos로 지정하기")]
    public void SetLeftTopCornerFromCurrentPos()
    {
        leftTopCornerPos = transform.position;
    }

    public void Push(GameObject obj)
    {
        int rowIndex = objectStack.Count % row;
        int colIndex = objectStack.Count / row;

        Vector3 pos = Vector3.right * rowIndex * space.x;
        pos += Vector3.up * colIndex * space.y;
        pos += leftTopCornerPos;
        obj.transform.position = pos;
        objectStack.Push(obj);
    }
    
    [Button("[TEST] 푸쉬")]
    public async void PushTest()
    {
        var obj = await Managers.Pool.PopAsync(poolId);
        obj.transform.SetParent(transform);
        obj.gameObject.SetActive(true);
        Push(obj.gameObject);
    }

    [Button("[TEST] 팝")]
    public GameObject Pop()
    {
        if (objectStack.Count == 0)
        {
            return null;
        }

        var obj = objectStack.Pop();
            return obj;
    }
}
