using System;
using Code;
using Ninject;
using Ninject.Activation;

public class GlobalConfiguration : Configuration
{
    [SelectImplementationOf(typeof (ICameraProvider))] public string CameraProviderImplementation;

    [SelectImplementationOf(typeof (IInputController))] public string InputControllerImplementation;

    [SelectImplementationOf(typeof (IMovementController))] public string MovementControllerImplementation;

    [SelectImplementationOf(typeof (IUnityAPI))] public string UnityAPIImplementation;

    public override void SetupBindings(
        IKernel kernel,
        Func<string, Type> lookupType,
        Func<IContext, object> currentGameScope)
    {
        kernel.Bind<IUnityAPI>().To(lookupType(UnityAPIImplementation)).InSingletonScope();
        kernel.Bind<IMovementController>().To(lookupType(MovementControllerImplementation)).InTransientScope();
        kernel.Bind<IInputController>().To(lookupType(InputControllerImplementation)).InTransientScope();
        kernel.Bind<ICameraProvider>().To(lookupType(CameraProviderImplementation)).InTransientScope();
    }
}