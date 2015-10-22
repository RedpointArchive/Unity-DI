using System;

namespace Code
{
    public abstract class DependencyTargetCustomEditor<T> : IDependencyTargetCustomEditor
    {
        public abstract void OnInspectorGUI(IEditableSettingsStoreAccessor accessor);

        public Type GetTargetType()
        {
            return typeof (T);
        }
    }
}
