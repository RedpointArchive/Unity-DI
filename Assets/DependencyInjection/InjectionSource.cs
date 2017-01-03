using System;
using UnityEngine;

[Serializable]
public class InjectionSource
{
    public string InjectionSourceName { get; set; }

    public Component[] ImpliedComponents { get; set; }
}