using System;
using UnityEngine;

public abstract class PurchasableItem : MonoBehaviour
{
    protected void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public abstract void OnPurchased();
}
