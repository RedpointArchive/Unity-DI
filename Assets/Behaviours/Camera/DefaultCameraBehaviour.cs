using UnityEngine;

public class DefaultCameraBehaviour : InjectedBehaviour, ICameraBehaviour
{
    public Camera Camera
    {
        get
        {
            return Camera.main;
        }
    }
}