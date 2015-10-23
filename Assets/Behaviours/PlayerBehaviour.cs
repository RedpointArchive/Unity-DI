using Ninject;

public class PlayerBehaviour : BaseBehaviour
{
    private IMovementController _movementController;
    
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