namespace HhhNetwork
{
    public abstract class NetIdMessageBase : DataMessage
    {
        public NetIdMessageBase(byte[] buffer)
            : base(buffer)
        {
        }

        public NetIdMessageBase(NetMessageType type)
            : base(type)
        {
        }

        public short netId
        {
            get;
            set;
        }

        // net id (2)
        protected override int GetByteSize()
        {
            return 2; // net id (2)
        }

        protected override void Serialize(NetSerializer s)
        {
            s.Write(this.netId);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.netId = s.ReadShort();
        }
    }
}