using UnityEngine;

public interface IMovementController
{
    void Update(GameObject player);
    bool ShouldSpawnPrefab();
}