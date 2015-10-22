using System.Collections;
using System.Collections.Generic;

namespace Code
{
    public interface IEditableSettingsStoreAccessor
    {
        IEnumerable<string> GetNames();
        T Get<T>(string id, T @default = default(T));
        void Set<T>(string name, T value);
        void Unset(string name);
    }
}
