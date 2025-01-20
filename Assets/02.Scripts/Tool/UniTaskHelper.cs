using Cysharp.Threading.Tasks;
using System;

public static class UniTaskHelper 
{
    public static Action<T> Action<T>(Func<T, UniTaskVoid> asyncAction) 
    {
        return (t1) => asyncAction(t1).Forget();
    }
}
