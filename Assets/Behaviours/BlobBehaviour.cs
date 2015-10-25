using Ninject;

public class BlobBehaviour : BaseBehaviour
{
    protected override void Inject(IKernel kernel)
    {
        // This will intentionally fail.
        kernel.Get<IHasComponentOnSameGameObject<PlayerBehaviour>>();
    }

    protected override void AwakeAfterInjection()
    {
    }
}