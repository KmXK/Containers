using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sources
{
    public class ContainerColumn : IEnumerable<ContainerData>
    {
        private readonly int _maxHeight;
        private readonly Stack<ContainerData> _containers;

        public ContainerColumn(int maxHeight)
        {
            _maxHeight = maxHeight;

            _containers = new Stack<ContainerData>(maxHeight);
        }

        public int Height => _containers.Count;

        internal void Place(ContainerData container)
        {
            if (!CanPlace(container))
                return;

            container.Column = this;
            _containers.Push(container);
        }

        internal bool CanTake(ContainerData container)
        {
            if (!_containers.Any())
                return false;

            return _containers.Peek() == container;
        }

        internal bool TryTake(ContainerData container)
        {
            if (!_containers.Any())
                return false;

            if (_containers.Peek() == container)
            {
                _containers.Pop();
                return true;
            }

            return false;
        }

        internal ContainerData Take()
        {
            if (!_containers.Any())
                throw new InvalidOperationException("No containers to take!");

            return _containers.Pop();
        }

        public ContainerData Top() => _containers.Peek();

        internal bool CheckContainerType(ContainerType type)
        {
            if (_containers.Any())
                return type == _containers.Peek().Type;
            return true;
        }

        public bool CanPlace(ContainerData container)
        {
            if (!CheckContainerType(container.Type))
                return false;

            if (_containers.Count == _maxHeight)
                return false;

            return true;
        }

        public IEnumerator<ContainerData> GetEnumerator()
        {
            var array = _containers.ToArray();
            for (var i = _containers.Count - 1; i >= 0; i--)
            {
                yield return array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}