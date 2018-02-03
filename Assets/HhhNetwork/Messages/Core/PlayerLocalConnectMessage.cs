namespace HhhNetwork
{
    public sealed class PlayerLocalConnectMessage : PlayerLocalConnectMessageDefault
    {
        public string name
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 1 + this.name.GetByteSize(); // string count (1) + string
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.name);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.name = s.ReadString();
        }
    }
}