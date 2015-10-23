using Code;
using UnityEngine;

public class ConfigurablePlayerStyle : IPlayerStyle
{
    [EditableSetting("material", "Player Material", null)]
    private readonly Material _playerMaterial;

    public ConfigurablePlayerStyle(ISettingsStoreAccessor settingsStoreAccessor)
    {
        _playerMaterial = settingsStoreAccessor.Get<ConfigurablePlayerStyle, Material>("material");
    }

    public Material PlayerMaterial
    {
        get { return _playerMaterial; }
    }
}