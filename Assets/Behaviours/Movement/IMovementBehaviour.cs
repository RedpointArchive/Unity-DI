using UnityEngine;

public interface IMovementBehaviour
{
    void Update(GameObject player);

    bool ShouldSpawnPrefab();
}
