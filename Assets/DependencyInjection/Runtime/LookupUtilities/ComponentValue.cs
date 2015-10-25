public class ComponentValue<T> : IHasComponentOnSameGameObject<T>, IHasComponentInChildren<T>, IHasComponentInParent<T>
{
    public ComponentValue(T value)
    {
        Component = value;
    }

    public T Component { get; private set; }
}