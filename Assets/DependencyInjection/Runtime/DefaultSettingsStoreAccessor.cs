using Ninject.Activation;
using Ninject.Planning.Bindings;

namespace Code
{
    public class DefaultSettingsStoreAccessor : ISettingsStoreAccessor
    {
        private readonly ISettingsStoreLoader _settingsStoreLoader;

        private readonly IContext _context;

        private readonly string _instanceName;

        public DefaultSettingsStoreAccessor(ISettingsStoreLoader settingsStoreLoader, IContext context)
        {
            _settingsStoreLoader = settingsStoreLoader;
            _context = context;
            _instanceName = null;

            // Traverse up the request context until we get to the root request, and then
            // check if there was a name associated with the request.  If there was, that name
            // is the name of the settings store instance.  So when you do 'kernel.Get<IBoatMovement>("test")'
            // that "test" is the name of the settings store instance, even if we're resolving
            // an interface 10 levels deep in the hierarchy.
            var currentRequest = _context.Request;
            while (currentRequest.ParentRequest != null) currentRequest = currentRequest.ParentRequest;

            if (currentRequest.Constraint == null)
            {
                return;
            }

            var bindingMetadata = new BindingMetadata();
            foreach (var instanceName in _settingsStoreLoader.InstanceNames)
            {
                bindingMetadata.Name = instanceName;
                if (currentRequest.Constraint(bindingMetadata))
                {
                    // The request matches on a specific name, so set the
                    // instance name field.
                    _instanceName = instanceName;
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the specified option.
        /// </summary>
        /// <param name="name">The unique ID of the option.</param>
        /// <typeparam name="TImplementor">The implementor type which this option is defined against.</typeparam>
        /// <typeparam name="TReturn">The type of the option value.</typeparam>
        /// <returns>The value of the option.</returns>
        public TReturn Get<TImplementor, TReturn>(string name)
        {
            return _settingsStoreLoader.Get<TImplementor, TReturn>(_instanceName, name);
        }
    }
}
