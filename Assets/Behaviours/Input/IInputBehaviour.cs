using UnityEngine;

public interface IInputBehaviour
{
    bool Clicked();

    Vector3 GetMousePosition();

    bool KeyPressed(KeyCode key);
}
