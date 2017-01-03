using UnityEngine;

public class UnityInputBehaviour : InjectedBehaviour, IInputBehaviour
{
    public bool Clicked()
    {
        return Input.GetMouseButtonDown(0);
    }

    public Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    public bool KeyPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }
}
