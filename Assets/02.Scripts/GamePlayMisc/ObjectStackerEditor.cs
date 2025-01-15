#if UNITY_EDITOR
using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectStacker))]
public class ObjectStackerEditor : Editor
{
    private ObjectStacker stacker;

    private void OnEnable()
    {
        stacker = target as ObjectStacker;
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        Vector3 pos = Handles.PositionHandle(stacker.leftTopCornerPos, quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            stacker.leftTopCornerPos = pos;
        }
        
        Handles.Label(pos, "LeftTopCorner");
    }
}
#endif
