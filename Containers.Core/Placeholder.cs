using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Containers.Core
{
    public class Placeholder
    {
        private readonly int _maxHeight;
        private readonly ContainerPlace[] _placements;

        private readonly List<ContainerColumn> _largeContainerColumns = new List<ContainerColumn>();
        private readonly List<ContainerColumn> _smallContainerColumns = new List<ContainerColumn>();
        private readonly Dictionary<int, ContainerData> _dict = new Dictionary<int, ContainerData>();

        private int _maxCountOfContainerPlaces;
        
        public int CountOfContainerPlaces { get; private set; }
        public ContainerPlace[] Placements => (ContainerPlace[]) _placements.Clone();

        public Placeholder(int maxHeight, int maxCountOfContainerPlaces)
        {
            _maxHeight = maxHeight;
            _maxCountOfContainerPlaces = maxCountOfContainerPlaces;
            _placements = new ContainerPlace[_maxCountOfContainerPlaces];
        }

        public void Place(ContainerData containerData, int containerPlaceIndex)
        {
            if (_placements[containerPlaceIndex] == null)
            {
                switch (containerData.Type)
                {
                    case ContainerType.Large:
                        CreatePlaceForLargeContainer(containerPlaceIndex);
                        break;
                    case ContainerType.Small:
                        CreatePlaceForSmallContainer(containerPlaceIndex);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                CountOfContainerPlaces++;
            }
            
            _placements[containerPlaceIndex].Place(containerData);
            
            _dict[containerData.Id] = containerData;
        }

        public ContainerData Find(int id)
        {
            return _dict[id];
        }

        public ContainerData Remove(ContainerData containerData)
        {
            var column = containerData.Column;
            var place = column.ContainerPlace;

            column.Take(containerData);

            if (place.Count == 0)
                _placements[place.IndexInPlaceholder] = null;

            return containerData;
        }

        private void CreatePlaceForSmallContainer(int containerPlaceIndex)
        {
            var first = new ContainerColumn(_maxHeight);
            var second = new ContainerColumn(_maxHeight);
            _smallContainerColumns.Add(first);
            _smallContainerColumns.Add(second);
            _placements[containerPlaceIndex] = new ContainerPlaceForSmall(first, second, containerPlaceIndex);
        }

        private void CreatePlaceForLargeContainer(int containerPlaceIndex)
        {
            var column = new ContainerColumn(_maxHeight);
            _largeContainerColumns.Add(column);
            _placements[containerPlaceIndex] = new ContainerPlaceForLarge(column, containerPlaceIndex);
        }
    }
}
