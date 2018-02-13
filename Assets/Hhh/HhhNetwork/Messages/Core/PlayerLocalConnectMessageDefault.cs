namespace HhhNetwork
{
    public class PlayerLocalConnectMessageDefault : DataMessage
    {
        public PlayerLocalConnectMessageDefault(byte[] buffer)
            : base(buffer)
        {
        }

        public PlayerLocalConnectMessageDefault()
            : base(NetMessageType.LocalConnect)
        {
        }

        public PlayerType playerType
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return 1; // player type (1)
        }

        protected override void Serialize(NetSerializer s)
        {
            s.Write((byte)this.playerType);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.playerType = (PlayerType)s.ReadByte();
        }
    }
}