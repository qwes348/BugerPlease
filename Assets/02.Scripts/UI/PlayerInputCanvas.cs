using UnityEngine;

public class PlayerInputCanvas : MonoBehaviour
{
    [SerializeField]
    private Joystick stick;

    public Vector3 GetInputDir()
    {
        return stick.Direction;
    }
}
