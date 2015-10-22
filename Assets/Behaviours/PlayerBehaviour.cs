using Ninject;

public class PlayerBehaviour : BaseBehaviour
{
    private IMovementController _movementController;

    /*public new void Start()
    {
        base.Start();
    }*/

    protected override void Inject(IKernel kernel)
    {
        _movementController = kernel.Get<IMovementController>();
    }

    protected override void StartAfterInjection()
    {
    }

    public void Update()
    {
        _movementController.Update(gameObject);
    }
}