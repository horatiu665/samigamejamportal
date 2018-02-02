namespace HhhNetwork
{
    using System.Collections.Generic;

    public static class NetDeserializerPool
    {
        private static Stack<NetDeserializer> dPool = new Stack<NetDeserializer>();

        public static NetDeserializer Get(byte[] buffer)
        {
            NetDeserializer deserializer;
            if (dPool.Count > 0)
            {
                deserializer = dPool.Pop();
            }
            else
            {
                deserializer = new NetDeserializer();
            }
            deserializer.SetBuffer(buffer);

            return deserializer;
        }

        public static void Return(NetDeserializer deserializer)
        {
            dPool.Push(deserializer);
        }
    }
}