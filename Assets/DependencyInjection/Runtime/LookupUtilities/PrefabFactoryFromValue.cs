using Ninject.Parameters;
using UnityEngine;

public class PrefabFactoryFromValue : Parameter
{
    public PrefabFactoryFromValue(Transform template)
        : base("prefab-factory-" + (template == null ? string.Empty : template.name), template, false)
    {
        Transform = template;
    }

    public Transform Transform { get; private set; }
}