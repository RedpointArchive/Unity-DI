using System;
using System.Linq;
using System.Reflection;
using Code;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using UnityEngine;

public class LookupModule : NinjectModule
{
    public override void Load()
    {
        Bind(typeof(IHasComponentOnSameGameObject<>)).ToMethod(ResolveComponentOnSameGameObject);
        Bind(typeof(IHasComponentInChildren<>)).ToMethod(ResolveComponentInChildren);
        Bind(typeof(IHasComponentInParent<>)).ToMethod(ResolveComponentInParent);
        Bind(typeof (IPrefabFactory)).ToMethod(context => ResolvePrefabFactory(context));
    } 

    public static object ResolvePrefabFactory(IContext context, Action<Transform> valueToTrack = null)
    {
        if (context.Kernel.Settings.Get<bool>("IsContextFreeVerification", false))
        {
            return new DefaultPrefabFactory(null);
        }
        else
        {
            var value = context.Parameters.OfType<PrefabFactoryFromValue>().FirstOrDefault();
            if (value == null)
            {
                throw new ActivationException("No PrefabFactoryFromValue set.");
            }

            if (value.Transform == null)
            {
                throw new ActivationException("No transform configuration for PrefabFactory.");
            }

            if (valueToTrack != null)
            {
                valueToTrack(value.Transform);
            }

            return new DefaultPrefabFactory(value.Transform);
        }
    }

    private object ResolveComponentInParent(IContext context)
    {
        return ResolveComponent(context, (o, type) => o.GetComponentInParent(type), "on parent of game object");
    }

    private object ResolveComponentOnSameGameObject(IContext context)
    {
        return ResolveComponent(context, (o, type) => o.GetComponent(type), "on game object");
    }

    private object ResolveComponentInChildren(IContext context)
    {
        return ResolveComponent(context, (o, type) => o.GetComponentInChildren(type), "on children of game object");
    }

    private object ResolveComponent(IContext context, Func<GameObject, Type, Component> lookup, string failContext)
    {
        if (context.Kernel.Settings.Get<bool>("IsContextFreeVerification", false))
        {
            var temp = new GameObject();
            var component = temp.AddComponent(context.Request.Service.GetGenericArguments()[0]);

            var valueType = typeof (ComponentValue<>).MakeGenericType(context.Request.Service.GetGenericArguments()[0]);
            var result = Activator.CreateInstance(valueType, new object[] {component});
            UnityEngine.Object.DestroyImmediate(temp);
            return result;
        }
        else
        {
            var current = context.Kernel.Settings.Get<GameObject>("CurrentGameObject", null);
            if (current == null)
            {
                throw new ActivationException("No current game object for IHasComponentOnSameGameObject<> request.");
            }

            var componentType = context.Request.Service.GetGenericArguments()[0];
            var component = lookup(current, componentType);
            if (component == null)
            {
                throw new ActivationException("Component of type " + componentType.FullName +
                                              " not found " + failContext + " '" + current.name + "'.");
            }

            var valueType = typeof (ComponentValue<>).MakeGenericType(componentType);
            return Activator.CreateInstance(valueType, new object[] {component});
        }
    }
}