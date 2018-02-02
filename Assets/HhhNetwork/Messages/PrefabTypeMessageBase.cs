namespace HhhNetwork
{
    public abstract class PrefabTypeMessageBase : DataMessage
    {
        public PrefabTypeMessageBase(NetMessageType type) : base(type)
        {
        }

        public PrefabTypeMessageBase(byte[] buffer) : base(buffer)
        {
        }

        //public VRPrefabType prefabType
        //{
        //    get;
        //    set;
        //}

        protected override int GetByteSize()
        {
            return 0/* this.prefabType.GetByteSize()*/;
        }

        protected override void Serialize(NetSerializer s)
        {
            //s.Write(this.prefabType);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            //this.prefabType = s.ReadPrefabType();
        }
    }
}