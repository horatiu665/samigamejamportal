namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;

    public class SpawnGrabRequestMessage : PrefabTypeMessageBase
    {
        public SpawnGrabRequestMessage(byte[] buffer) : base(buffer)
        {
        }

        public SpawnGrabRequestMessage() : base(NetMessageType.SpawnGrabRequest)
        {
        }

        public bool isLeft
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 1;
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(isLeft);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.isLeft = s.ReadBool();
        }
    }
}