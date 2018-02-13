namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;

    public class RigidbodySyncSpawnMessage : DataMessage
    {
        public RigidbodySyncSpawnMessage(byte[] buffer) : base(buffer)
        {
        }

        public RigidbodySyncSpawnMessage() : base(NetMessageType.RigidbodySyncSpawn)
        {
        }

        public RigidbodySyncData data
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return RigidbodySyncUpdateMessage.bytesPerData;
        }

        protected override void Serialize(NetSerializer s)
        {
            s.Write(this.data.syncId);
            s.Write(this.data.prefabType);
            s.WriteExact(this.data.position);
            s.Write(this.data.rotation);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.data = new RigidbodySyncData()
            {
                syncId = s.ReadInt(),
                prefabType = s.ReadPrefabType(),
                position = s.ReadExactVector3(),
                rotation = s.ReadQuaternion()
            };
        }
    }
}