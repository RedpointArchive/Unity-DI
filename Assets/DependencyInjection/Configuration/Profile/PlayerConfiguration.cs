using System;
using Code;
using Ninject;
using Ninject.Activation;

public class PlayerConfiguration : Configuration
{
    [SelectImplementationOf(typeof (IPlayerStyle))] public string PlayerStyleImplementation;

    [SettingInstanceName] public string ProfileName;

    public override void SetupBindings(
        IKernel kernel,
        Func<string, Type> lookupType,
        Func<IContext, object> currentGameScope)
    {
        kernel.Bind<IPlayerStyle>().To(lookupType(PlayerStyleImplementation)).Named(ProfileName);
    }
}