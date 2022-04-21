﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers.Core
{
    public class ContainerPlaceForLarge : ContainerPlace
    {
        private readonly ContainerColumn _column;

        public ContainerPlaceForLarge(ContainerColumn column, int index) : base(index)
        {
            _column = column;
        }

        public override void Place(ContainerData containerData)
        {
            if (containerData.Type == ContainerType.Small)
                throw new ArgumentException("Typecast");

            _column.Place(containerData);
        }

        public override int Count => _column.Height;

        public ContainerColumn Column => _column;
    }
}
