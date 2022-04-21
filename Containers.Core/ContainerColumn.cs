using System.Collections;

namespace Containers.Core;

public class ContainerColumn : IEnumerable<ContainerData>
{
    private readonly int _maxHeight;
    private readonly Stack<ContainerData> _containers;

    public int Height => _containers.Count;
    public ContainerPlace ContainerPlace { get; private set; }

    public ContainerColumn(int maxHeight)
    {
        _maxHeight = maxHeight;
        _containers = new Stack<ContainerData>(maxHeight);
    }

    public void Place(ContainerData containerData)
    {
        if (Height == _maxHeight)
            throw new ArgumentOutOfRangeException("PlacesCount");

        _containers.Push(containerData);
    }

    public ContainerData Take(ContainerData containerData)
    {
        if (Height <= 0)
            throw new ArgumentOutOfRangeException("PlacesCount");

        if (Peek() != containerData)
            throw new InvalidOperationException();

        return _containers.Pop();
    }

    public ContainerData Peek()
    {
        if (Height <= 0)
            throw new ArgumentOutOfRangeException("PlacesCount");

        return _containers.Peek();
    }
    
    public IEnumerator<ContainerData> GetEnumerator()
    {
        return _containers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}