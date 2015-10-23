using Code;
using Ninject;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerBehaviour : BaseBehaviour
{
    private IMovementController _movementController;

    private IPlayerStyle _playerStyle;

    [SettingInstanceNameSelector(typeof(PlayerConfiguration))]
    public string PlayerStyle;

    protected override void Inject(IKernel kernel)
    {
        _movementController = kernel.Get<IMovementController>();
        _playerStyle = kernel.Get<IPlayerStyle>(PlayerStyle);
    }

    protected override void AwakeAfterInjection()
    {
        // NOTE: It'd be great if we could dependency inject
        // the MeshRenderer, but we need support from
        // Unity Tech to do this.
        GetComponent<MeshRenderer>().material = _playerStyle.PlayerMaterial;
    }

    public void Update()
    {
        _movementController.Update(gameObject);
    }
}