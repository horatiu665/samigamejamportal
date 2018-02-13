namespace HhhNetwork
{
    public class PlayerDisconnectMessageDefault : NetIdMessageBase
    {
        public PlayerDisconnectMessageDefault(byte[] buffer)
            : base(buffer)
        {
        }

        public PlayerDisconnectMessageDefault()
            : base(NetMessageType.Disconnect)
        {
        }
    }
}