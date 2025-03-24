using System;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class TmpWobble : MonoBehaviour
{
    public float wobbleAmount = 10f;
    public float wobbleSpeed = 5f;
    
    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (textComponent == null)
            return;
        
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if(!charInfo.isVisible)
                continue;
            
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var origin = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = origin + Vector3.up * (Mathf.Sin(Time.time * wobbleSpeed + origin.x + 0.01f) * wobbleAmount);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
