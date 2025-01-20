using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameUI : StaticMono<GameUI>
{
    [SerializeField]
    private PlayerInputCanvas inputCanvas;
    
    public PlayerInputCanvas InputCanvas => inputCanvas;
}
