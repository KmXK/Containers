namespace Containers.Core;

public abstract class ContainerPlace
{
    protected ContainerPlace(int indexInPlaceholder)
    {
        IndexInPlaceholder = indexInPlaceholder;
    }
    
    public abstract void Place(ContainerData containerData);

    public abstract int Count { get; }

    public int IndexInPlaceholder { get; }
}