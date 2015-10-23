using UnityEngine;
using System.Collections;

public class UseArrowKeysMovementController : IMovementController
{
    private readonly IInputController _inputController;

    public UseArrowKeysMovementController(
        IInputController inputController)
    {
        _inputController = inputController;
    }

    public void Update(GameObject player)
    {
        if (_inputController.KeyPressed(KeyCode.UpArrow))
        {
            player.transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z + 0.1f);
        }

        if (_inputController.KeyPressed(KeyCode.DownArrow))
        {
            player.transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z - 0.1f);
        }
        if (_inputController.KeyPressed(KeyCode.LeftArrow))
        {
            player.transform.position = new Vector3(
                player.transform.position.x - 0.1f,
                player.transform.position.y,
                player.transform.position.z);
        }

        if (_inputController.KeyPressed(KeyCode.RightArrow))
        {
            player.transform.position = new Vector3(
                player.transform.position.x + 0.1f,
                player.transform.position.y,
                player.transform.position.z);
        }
    }
}
