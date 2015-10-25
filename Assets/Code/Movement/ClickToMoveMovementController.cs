using UnityEngine;

public class ClickToMoveMovementController : IMovementController
{
    private readonly IInputController _inputController;
    private readonly ICameraProvider _cameraProvider;

    private Vector3? _targetLocation;

    private readonly Plane _groundPlane = new Plane(
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1));

    public ClickToMoveMovementController(
        IInputController inputController,
        ICameraProvider cameraProvider)
    {
        _inputController = inputController;
        _cameraProvider = cameraProvider;
    }

    public void Update(GameObject player)
    {
        if (_inputController.Clicked())
        {
            var ray = _cameraProvider.Camera.ScreenPointToRay(_inputController.GetMousePosition());
            float enter;
            if (_groundPlane.Raycast(ray, out enter))
            {
                _targetLocation = ray.GetPoint(enter);
            }
        }

        if (_targetLocation != null)
        {
            player.transform.position = Vector3.Lerp(
                player.transform.position,
                _targetLocation.Value,
                0.1f);
        }
    }

    public bool ShouldSpawnPrefab()
    {
        return _inputController.Clicked();
    }
}