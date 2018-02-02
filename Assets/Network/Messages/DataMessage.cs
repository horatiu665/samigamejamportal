namespace HhhNetwork
{
    /// <summary>
    /// Abstract base class representing all LLAPI messages.
    /// </summary>
    public abstract class DataMessage
    {
        protected NetMessageType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMessage"/> class, by deserializing from the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public DataMessage(byte[] buffer)
        {
            Deserialize(buffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMessage"/> class, which may be serialized after all data is set.
        /// </summary>
        /// <param name="type">The type.</param>
        public DataMessage(NetMessageType type)
        {
            _type = type;
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public NetMessageType type
        {
            get { return _type; }
        }

        /// <summary>
        /// Serializes this message using the given <see cref="NetSerializer"/>.
        /// </summary>
        protected abstract void Serialize(NetSerializer s);

        /// <summary>
        /// Deserializes this message using the given <see cref="NetDeserializer"/>.
        /// </summary>
        protected abstract void Deserialize(NetDeserializer s);

        /// <summary>
        /// Returns the byte size of this message, excluding the MessageType (1 byte).
        /// </summary>
        protected abstract int GetByteSize();

        /// <summary>
        /// Gets the total size of this message in bytes, including the MessageType (1 byte).
        /// </summary>
        /// <returns></returns>
        public int GetTotalByteSize()
        {
            // 1 byte is for the type, which is always the first byte
            return 1 + this.GetByteSize();
        }

        /// <summary>
        /// Serializes this data message, automatically creating or resizing the <see cref="NetSerializer"/> buffer as needed. Automatically writes the MessageType as the first byte.
        /// </summary>
        /// <returns>A byte array buffer populated with the data for this message.</returns>
        public byte[] Serialize()
        {
            var serializer = NetSerializer.instance;
            serializer.EnsureBufferSize(GetTotalByteSize());
            serializer.ResetIndex();

            serializer.Write(_type);
            Serialize(serializer);
            return serializer.buffer;
        }

        /// <summary>
        /// Deserializes this data message using the <see cref="NetDeserializer"/>. Automatically writes the MessageType as the first byte.
        /// </summary>
        public void Deserialize(byte[] buffer)
        {
            var deserializer = NetDeserializerPool.Get(buffer);

            _type = deserializer.ReadType();
            Deserialize(deserializer);

            NetDeserializerPool.Return(deserializer);
        }
    }
}