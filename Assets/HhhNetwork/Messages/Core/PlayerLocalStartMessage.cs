namespace HhhNetwork
{
    using UnityEngine;

    public sealed class PlayerLocalStartMessage : PlayerLocalStartMessageDefault
    {
        public Color color
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 6; // color.rgb halfs (6)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.color, false);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.color = s.ReadColor(false);
        }
    }
}