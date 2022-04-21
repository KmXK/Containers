using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers.Core
{
    public class ContainerPlaceForSmall : ContainerPlace
    {
        private readonly ContainerColumn _firstColumn;
        private readonly ContainerColumn _secondColumn;

        public ContainerPlaceForSmall (ContainerColumn firstColumn, ContainerColumn secondColumn, int index) : base(index)
        {
            _firstColumn = firstColumn;
            _secondColumn = secondColumn;
        }

        public override void Place(ContainerData containerData)
        {
            if (containerData.Type == ContainerType.Large)
                throw new ArgumentException("Typecast");

            (_firstColumn.Height <= _secondColumn.Height ? _firstColumn : _secondColumn).Place(containerData);
        }

        public override int Count => _firstColumn.Height + _secondColumn.Height;

        public ContainerColumn FirstColumn => _firstColumn;
        public ContainerColumn SecondColumn => _secondColumn;
        
    }
}
