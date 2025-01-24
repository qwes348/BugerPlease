using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameUI : StaticMono<GameUI>
{
    [SerializeField]
    private PlayerInputCanvas inputCanvas;
    [SerializeField]
    private SpeechBubbleCanvas speechBubbleCanvas;
    [SerializeField]
    private HudCanvas hudCanvas;
    
    public PlayerInputCanvas InputCanvas => inputCanvas;
    public SpeechBubbleCanvas SpeechBubbleCanvas => speechBubbleCanvas;
    public HudCanvas HudCanvas => hudCanvas;
}
