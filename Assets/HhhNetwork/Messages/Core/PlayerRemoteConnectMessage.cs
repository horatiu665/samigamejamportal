namespace HhhNetwork
{
    using UnityEngine;

    public class PlayerRemoteConnectMessage : PlayerRemoteConnectMessageDefault
    {
        public Color color
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 6 + 1 + name.GetByteSize();  // color.rgb (6) + string (1 + length)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.color, false);
            s.Write(this.name);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.color = s.ReadColor(false);
            this.name = s.ReadString();
        }
    }
}