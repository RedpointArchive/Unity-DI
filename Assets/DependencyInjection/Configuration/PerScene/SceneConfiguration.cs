using System;
using Ninject;
using Ninject.Activation;

namespace Code
{
    public class SceneConfiguration : Configuration
    {
        public override bool PerScene
        {
            get { return true; }
        }
        
        [SelectImplementationOf(typeof(IMovementController))]
        public string MovementControllerImplementation;

        public override void SetupBindings(IKernel kernel, Func<string, Type> lookupType, Func<IContext, object> currentGameScope)
        {
            if (!string.IsNullOrEmpty(this.MovementControllerImplementation))
            {
                kernel.Rebind<IMovementController>()
                    .To(lookupType(this.MovementControllerImplementation))
                    .InTransientScope();
            }
        }
    }
}
