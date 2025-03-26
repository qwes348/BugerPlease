using UnityEngine;

[CreateAssetMenu(fileName = "FieldItemData_ExtraTime", menuName = "Scriptable Objects/FieldItemData_ExtraTime")]
public class FieldItemData_ExtraTime : FieldItemData
{
    [SerializeField]
    private float extraTime;
    
    public float ExtraTime => extraTime;
}
