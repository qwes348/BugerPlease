using UnityEngine;
using UnityEngine.Serialization;

public class Stackable : MonoBehaviour
{
    [SerializeField]
    private Define.StackableType myStackableType;
    
    public Define.StackableType MyStackableType => myStackableType;
}
