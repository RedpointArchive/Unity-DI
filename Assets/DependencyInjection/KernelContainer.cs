using Protoinject;
using UnityEngine;

public class KernelContainer
{
    public static KernelContainer _instance;

    public static KernelContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KernelContainer();
            }

            return _instance;
        }
    }

    public KernelContainer(bool isVerifier = false)
    {
        Kernel = new StandardKernel();

        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Configuration"))
        {
            var configuration = gameObject.GetComponent<Configuration>();

            configuration.Configure(Kernel);
        }
    }

    public StandardKernel Kernel { get; private set; }
}
