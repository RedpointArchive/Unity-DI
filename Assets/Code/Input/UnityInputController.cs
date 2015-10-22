using UnityEngine;

public class UnityInputController : IInputController
{
    public bool Clicked()
    {
        return Input.GetMouseButtonDown(0);
    }

    public Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }
}