using System;

namespace Sources
{
    public class ContainerPlace
    {
        private readonly ContainerColumn _firstColumn;
        private readonly ContainerColumn _secondColumn;

        public ContainerPlace(int maxHeight)
        {
            _firstColumn = new ContainerColumn(maxHeight);
            _secondColumn = new ContainerColumn(maxHeight);
        }

        public void Place(ContainerData container, ContainerColumn column = null)
        {
            if (!CheckContainerType(container.Type))
                throw new ArgumentException("Invalid container type!");

            if (column != null && column != _firstColumn && column != _secondColumn)
                throw new ArgumentException("Invalid column!");

            switch (container.Type)
            {
                case ContainerType.Small:
                    PlaceSmall(container, column);
                    break;
                case ContainerType.Large:
                    PlaceLarge(container);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool TryTake(ContainerData container)
        {
            return _firstColumn.TryTake(container) || _secondColumn.TryTake(container);
        }

        public bool CheckContainerType(ContainerType type)
        {
            return _firstColumn.CheckContainerType(type) && _secondColumn.CheckContainerType(type);
        }

        public (int Column, int Height) GetColumnInfo(ContainerColumn column)
        {
            if (column == _firstColumn)
                return (0, _firstColumn.Height);
            else
                return (1, _secondColumn.Height);
        }

        private void PlaceSmall(ContainerData container, ContainerColumn column)
        {
            if(column != null)
            {
                column.Place(container);
            }
            else
            {
                if (_firstColumn.Height <= _secondColumn.Height)
                    _firstColumn.Place(container);
                else
                    _secondColumn.Place(container);
            }
        }
        
        private void PlaceLarge(ContainerData container)
        {
            _firstColumn.Place(container);
        }
    }
}