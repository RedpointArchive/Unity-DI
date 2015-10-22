using Code;
using Ninject;
using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    public void Start()
    {
        Inject(KernelContainer.Kernel);
        StartAfterInjection();
    }

    protected abstract void Inject(IKernel kernel);

    protected abstract void StartAfterInjection();
}