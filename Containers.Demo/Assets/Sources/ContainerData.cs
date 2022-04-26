using System;

namespace Sources
{
    [Serializable]
    public class ContainerData
    {
        public ContainerType Type;
        public int Id;
        public string Company;
        public string SenderCountry;
        
        public ContainerColumn Column;
        public Container Container;
    }
}