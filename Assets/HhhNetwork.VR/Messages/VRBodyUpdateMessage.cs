namespace HhhNetwork
{
    public class VRBodyUpdateS2CMessage : VRBodyUpdateC2SMessage
    {
        public VRBodyUpdateS2CMessage()
            : this(NetMessageType.VRBodyUpdateS2C)
        {
        }

        public VRBodyUpdateS2CMessage(NetMessageType messageType)
            : base(messageType)
        {
        }

        public VRBodyUpdateS2CMessage(byte[] buffer)
            : base(buffer)
        {
        }

        public short netId
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 2; // player net id (2)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.netId);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.netId = s.ReadShort();
        }
    }
}