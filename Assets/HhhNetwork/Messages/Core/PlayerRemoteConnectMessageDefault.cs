namespace HhhNetwork
{
    using UnityEngine;

    public class PlayerRemoteConnectMessageDefault : NetIdMessageBase
    {
        public PlayerRemoteConnectMessageDefault(byte[] buffer)
            : base(buffer)
        {
        }

        public PlayerRemoteConnectMessageDefault()
            : base(NetMessageType.RemoteConnect)
        {
        }

        public PlayerType playerType
        {
            get;
            set;
        }

        public Vector3 position
        {
            get;
            set;
        }

        public Vector3 originShift
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 13; // player type (1) + position (6) + os (6)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write((byte)this.playerType);
            s.Write(this.position);
            s.Write(this.originShift);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.playerType = (PlayerType)s.ReadByte();
            this.position = s.ReadVector3();
            this.originShift = s.ReadVector3();
        }
    }
}