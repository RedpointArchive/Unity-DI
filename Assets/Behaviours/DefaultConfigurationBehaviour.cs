using Protoinject;

public class DefaultConfigurationBehaviour : Configuration
{
    public override void Configure(StandardKernel kernel)
    {
        kernel.Bind<ICameraBehaviour>().To<DefaultCameraBehaviour>();
        kernel.Bind<IInputBehaviour>().To<UnityInputBehaviour>();
        kernel.Bind<IMovementBehaviour>().To<ClickToMoveMovementBehaviour>();
    }
}
