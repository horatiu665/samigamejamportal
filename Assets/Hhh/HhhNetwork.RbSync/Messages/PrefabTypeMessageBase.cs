namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;

    public abstract class PrefabTypeMessageBase : DataMessage
    {
        public PrefabTypeMessageBase(NetMessageType messageType) : base(messageType)
        {
        }

        public PrefabTypeMessageBase(byte[] buffer) : base(buffer)
        {
        }

        public PrefabType prefabType
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return ((PrefabType)0).GetByteSize();
        }

        protected override void Serialize(NetSerializer s)
        {
            s.Write(this.prefabType);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.prefabType = s.ReadPrefabType();
        }
    }
}