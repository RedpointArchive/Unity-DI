using UnityEngine;

public class DefaultCameraProvider : ICameraProvider
{
    public Camera Camera
    {
        get
        {
            return Camera.main;
        }
    }
}