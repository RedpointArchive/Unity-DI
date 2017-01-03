using UnityEngine;

public class PlayerBehaviour : InjectedBehaviour
{
    private readonly IMovementBehaviour _movementBehaviour;

    public PlayerBehaviour(
        [FromParent] IMovementBehaviour movementBehaviour,
        [FromParent] MeshRenderer meshRenderer)
    {
        _movementBehaviour = movementBehaviour;
    }

    public void Update()
    {
        //_movementBehaviour.Update(gameObject);
    }
}
