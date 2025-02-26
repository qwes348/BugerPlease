using System;
using UnityEngine;

[RequireComponent(typeof(ObjectStacker))]
public class Dumpster : MonoBehaviour
{
    private ObjectStacker stacker;

    private void Awake()
    {
        stacker = GetComponent<ObjectStacker>();
        stacker.onPushed += OnTrashPushed;
    }

    private void OnTrashPushed()
    {
        var trash = stacker.Pop().GetComponent<Poolable>();
        Managers.Pool.Push(trash);
    }

    public void PushTrashManually(Stackable trash)
    {
        stacker.Push(trash);
    }
}
