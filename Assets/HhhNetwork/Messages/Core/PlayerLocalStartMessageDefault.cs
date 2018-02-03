namespace HhhNetwork
{
    using UnityEngine;

    public class PlayerLocalStartMessageDefault : NetIdMessageBase
    {
        public PlayerLocalStartMessageDefault()
            : base(NetMessageType.LocalStart)
        {
        }

        public PlayerLocalStartMessageDefault(byte[] buffer)
            : base(buffer)
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

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 7; // player type (1) + position (6)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write((byte)this.playerType);
            s.Write(this.position);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.playerType = (PlayerType)s.ReadByte();
            this.position = s.ReadVector3();
        }
    }
}