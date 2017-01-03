using Protoinject;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class InjectedSceneBehaviour : MonoBehaviour
{
    public void Update()
    {
        if (!Application.isEditor) return;

        Debug.Log("Validating scene...");

        var kernel = new KernelContainer(true);
        var sceneRootNode = kernel.Kernel.CreateEmptyNode("Scene");

        var scenePlans = CreateScenePlans(kernel.Kernel, sceneRootNode);
        try
        {
            kernel.Kernel.ValidateAll(scenePlans);
            Debug.Log(sceneRootNode.GetDebugRepresentation());
            Debug.Log("Scene passed validation.");

            // Go through the plan results and find any game objects that need
            // to be instantiated.
            foreach (var plan in scenePlans)
            {
                ProcessGameObject(kernel.Kernel, sceneRootNode, plan);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            kernel.Kernel.DiscardAll(scenePlans);
        }
    }

    private void ProcessGameObject(IKernel kernel, IPlan sceneRootNode, IPlan plan)
    {
        if (plan.RequestedType == typeof(GameObject))
        {
            foreach (var child in plan.ChildrenPlan.ToList())
            {
                if (child.RequestedType == typeof(GameObject))
                {
                    ProcessGameObject(kernel, sceneRootNode, child);
                }
                else
                {
                    // Temporarily disconnect this planned node from the hierarchy so that we can resolve it's dependencies properly.
                    kernel.Hierarchy.RemoveChildNode(child.Parent, child);

                    IPlan testPlan = null;
                    try
                    {
                        Debug.Log("Testing construction of component " + child.GetDebugRepresentation());

                        testPlan = kernel.Plan(
                            child.Type,
                            (INode)child.Parent,
                            null,
                            null,
                            null,
                            null,
                            null,
                            new Dictionary<Type, List<IMapping>>());

                        var existingChild = (InjectedBehaviour)child.PlannedMethod(null);
                        if (existingChild.InjectionSource == null)
                        {
                            existingChild.InjectionSource = new InjectionSource();
                        }
                        if (existingChild.InjectionSource.ImpliedComponents == null)
                        {
                            existingChild.InjectionSource.ImpliedComponents = new Component[0];
                        }

                        var addedComponents = new List<Component>();

                        foreach (var notInstantiatedChildPlan in testPlan.PlannedCreatedNodes)
                        {
                            if (notInstantiatedChildPlan == testPlan)
                            {
                                continue;
                            }

                            var parent = notInstantiatedChildPlan.ParentPlan;
                            if (parent.RequestedType == typeof(GameObject) && parent.PlannedMethod != null)
                            {
                                var gameObject = (GameObject)parent.PlannedMethod(null);

                                var component = gameObject.AddComponent(((INode)notInstantiatedChildPlan).Type);
                                addedComponents.Add(component);
                            }
                        }

                        existingChild.InjectionSource.ImpliedComponents = existingChild.InjectionSource.ImpliedComponents.Concat(addedComponents).Where(x => x != null).ToArray();
                    }
                    finally
                    {
                        if (testPlan != null)
                        {
                            kernel.Discard(testPlan);
                        }

                        kernel.Hierarchy.AddChildNode(child.Parent, child);
                    }
                }
            }
        }
    }

    public List<IPlan> CreateScenePlans(IKernel kernel, INode sceneRootNode)
    {
        var plans = new List<IPlan>();

        var rootObjects = new List<GameObject>();
        var scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        // iterate root objects and do something
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            var gameObject = rootObjects[i];

            var gameObjectPlan = kernel.Plan(
                typeof(GameObject),
                (INode)sceneRootNode,
                null,
                gameObject.name,
                (INode)sceneRootNode,
                null,
                null,
                new Dictionary<Type, List<IMapping>>
                {
                    {
                        typeof(GameObject), new List<IMapping>
                        {
                            new DefaultMapping(
                                null,
                                ctx => gameObject,
                                false,
                                null,
                                null,
                                true,
                                true,
                                null)
                        }
                    }
                });

            kernel.Hierarchy.AddChildNode(sceneRootNode, (INode)gameObjectPlan);

            AppendComponentPlans(kernel, sceneRootNode, gameObject, gameObjectPlan);
            AppendChildGameObjectPlans(kernel, sceneRootNode, gameObject, gameObjectPlan);

            plans.Add(gameObjectPlan);
        }

        return plans;
    }

    private void AppendComponentPlans(IKernel kernel, INode sceneRootNode, GameObject gameObject, IPlan gameObjectPlan)
    {
        var components = gameObject.GetComponents<InjectedBehaviour>();
        if (components == null)
        {
            return;
        }

        foreach (var component in components)
        {
            var componentPlan = kernel.Plan(
                component.GetType(),
                (INode)gameObjectPlan,
                null,
                null,
                (INode)sceneRootNode,
                null,
                null,
                new Dictionary<Type, List<IMapping>>
                {
                    {
                        component.GetType(), new List<IMapping>
                        {
                            new DefaultMapping(
                                null,
                                ctx => component,
                                false,
                                null,
                                null,
                                true,
                                true,
                                null)
                        }
                    }
                });
            ((INode)componentPlan).Name = "-EXISTING-";

            kernel.Hierarchy.AddChildNode(gameObjectPlan, (INode) componentPlan);
        }
    }

    private void AppendChildGameObjectPlans(IKernel kernel, INode sceneRootNode, GameObject gameObject, IPlan gameObjectPlan)
    {
        var count = gameObject.transform.childCount;
        for (var i = 0; i < count; i++)
        {
            var childGameObject = gameObject.transform.GetChild(i);

            var childGameObjectPlan = kernel.Plan(
                typeof(GameObject),
                (INode)gameObjectPlan,
                null,
                null,
                (INode)sceneRootNode,
                null,
                null,
                new Dictionary<Type, List<IMapping>>
                {
                    {
                        typeof(GameObject), new List<IMapping>
                        {
                            new DefaultMapping(
                                null,
                                ctx => childGameObject,
                                false,
                                null,
                                null,
                                true,
                                true,
                                null)
                        }
                    }
                });

            kernel.Hierarchy.AddChildNode(gameObjectPlan, (INode)childGameObjectPlan);
        }
    }
}
