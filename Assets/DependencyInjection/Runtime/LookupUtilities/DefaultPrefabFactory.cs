using UnityEngine;

public class DefaultPrefabFactory : IPrefabFactory
{
    private readonly Transform _template;

    public DefaultPrefabFactory(Transform template)
    {
        _template = template;
    }

    public Transform Instantiate()
    {
        return Object.Instantiate(_template);
    }
}