using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private static GameUI instance;
    public static GameUI Instance
    {
        get
        {
            if(instance == null)
                instance = FindAnyObjectByType<GameUI>();
            return instance;
        }
    }
    
    [SerializeField]
    private PlayerInputCanvas inputCanvas;
    
    public PlayerInputCanvas InputCanvas => inputCanvas;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
