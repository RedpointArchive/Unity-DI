using Protoinject;
using UnityEngine;

public abstract class Configuration : MonoBehaviour
{
    public abstract void Configure(StandardKernel kernel);
}