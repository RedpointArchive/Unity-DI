using UnityEngine;

public interface IInputController
{
    bool Clicked();
    Vector3 GetMousePosition();
    bool KeyPressed(KeyCode key);
}