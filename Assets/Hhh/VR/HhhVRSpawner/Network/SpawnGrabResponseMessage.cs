namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;

    public class SpawnGrabResponseMessage : PrefabTypeMessageBase
    {
        public SpawnGrabResponseMessage(byte[] buffer) : base(buffer)
        {
        }

        public SpawnGrabResponseMessage() : base(NetMessageType.SpawnGrabResponse)
        {
        }

        public bool isLeft
        {
            get;
            set;
        }

        public int syncId
        {
            get;
            set;
        }

        public short playerId
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 1 + 4 + 2;
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.isLeft);
            s.Write(this.syncId);
            s.Write(this.playerId);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.isLeft = s.ReadBool();
            this.syncId = s.ReadInt();
            this.playerId = s.ReadShort();
        }
    }
}