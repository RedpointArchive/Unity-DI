using UnityEngine;

public class InjectedBehaviour : MonoBehaviour
{
    public InjectionSource InjectionSource;

    public void Awake()
    {

    }

    public void Update()
    {
       /* if (!Application.isEditor) return;

        var fullType = GetType();
        var plan = KernelContainer.Instance.Kernel.Plan(fullType, null, null, null, null, new IConstructorArgument[0]);
        try
        {
            var children = plan.ChildrenPlan;
            foreach (var child in children)
            {
                if (child.)
            }
        }
        finally
        {
            KernelContainer.Instance.Kernel.Discard(plan);
        }*/
    }
}
