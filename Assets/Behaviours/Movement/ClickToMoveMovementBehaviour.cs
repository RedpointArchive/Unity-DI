using UnityEngine;

public class ClickToMoveMovementBehaviour : InjectedBehaviour, IMovementBehaviour
{
    private readonly IInputBehaviour _inputBehaviour;
    private readonly ICameraBehaviour _cameraBehaviour;

    private Vector3? _targetLocation;

    private readonly Plane _groundPlane = new Plane(
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1));

    public ClickToMoveMovementBehaviour(
        [FromParent] IInputBehaviour inputBehaviour,
        [FromParent] ICameraBehaviour cameraBehaviour)
    {
        _inputBehaviour = inputBehaviour;
        _cameraBehaviour = cameraBehaviour;
    }

    public void Update(GameObject player)
    {
        if (_inputBehaviour.Clicked())
        {
            var ray = _cameraBehaviour.Camera.ScreenPointToRay(_inputBehaviour.GetMousePosition());
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
        return _inputBehaviour.Clicked();
    }
}
