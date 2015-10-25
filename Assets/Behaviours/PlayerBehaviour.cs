using Code;
using Ninject;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerBehaviour : BaseBehaviour
{
    private IMovementController _movementController;

    private IPlayerStyle _playerStyle;

    private MeshRenderer _meshRenderer;

    private IPrefabFactory _blobFactory;

    [SettingInstanceNameSelector(typeof(PlayerConfiguration))]
    public string PlayerStyle;

    public Transform BlobPrefab;

    protected override void Inject(IKernel kernel)
    {
        _movementController = kernel.Get<IMovementController>();
        _playerStyle = kernel.Get<IPlayerStyle>(PlayerStyle);
        _meshRenderer = kernel.Get<IHasComponentOnSameGameObject<MeshRenderer>>().Component;
        _blobFactory = kernel.Get<IPrefabFactory>(new PrefabFactoryFromValue(BlobPrefab));
    }

    protected override void AwakeAfterInjection()
    {
        _meshRenderer.material = _playerStyle.PlayerMaterial;
    }

    public void Update()
    {
        _movementController.Update(gameObject);

        if (_movementController.ShouldSpawnPrefab())
        {
            var blob = _blobFactory.Instantiate();
            blob.transform.position = gameObject.transform.position;
        }
    }
}