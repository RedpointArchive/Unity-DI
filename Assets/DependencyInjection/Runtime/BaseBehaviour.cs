using Code;
using Ninject;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public void Awake()
    {
        if (!KernelContainer.IsForDependencyInjectionVerification)
        {
            KernelContainer.GameKernelContainer.Kernel.Settings.Set("CurrentGameObject", gameObject);
            Inject(KernelContainer.GameKernelContainer.Kernel);
            KernelContainer.GameKernelContainer.Kernel.Settings.Set("CurrentGameObject", null);
            AwakeAfterInjection();
        }
    }

    protected abstract void Inject(IKernel kernel);

    protected abstract void AwakeAfterInjection();
}