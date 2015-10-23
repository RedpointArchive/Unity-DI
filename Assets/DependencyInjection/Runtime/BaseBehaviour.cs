using Code;
using Ninject;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public void Awake()
    {
        if (!IsForDependencyInjectionVerification)
        {
            Inject(KernelContainer.GameKernelContainer.Kernel);
            AwakeAfterInjection();
        }
    }
    
    public static bool IsForDependencyInjectionVerification { get; set; }

    protected abstract void Inject(IKernel kernel);

    protected abstract void AwakeAfterInjection();
}