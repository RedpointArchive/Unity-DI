using System;

namespace Code
{
    public interface IDependencyTargetCustomEditor
    {
        void OnInspectorGUI(IEditableSettingsStoreAccessor accessor);

        Type GetTargetType();
    }
}
