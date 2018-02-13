namespace HhhNetwork
{
    /// <summary>
    /// Static helper class for getting typically used byte sizes for Unity engine types.
    /// </summary>
    public static class ByteSize
    {
        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Vector2"/> using halves.
        /// </summary>
        public const int Vector2Half = 4;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Vector2"/> using floats.
        /// </summary>
        public const int Vector2Exact = 8;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Vector3"/> using halves.
        /// </summary>
        public const int Vector3Half = 6;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Vector3"/> using floats.
        /// </summary>
        public const int Vector3Exact = 12;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Quaternion"/> using halves.
        /// </summary>
        public const int QuaternionHalf = 8;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Quaternion"/> using floats.
        /// </summary>
        public const int QuaternionExact = 16;

        /// <summary>
        /// Size in bytes of <see cref="UnityEngine.Color"/> using halves. This is the default ReadColor() and WriteColor() in the Serializer, contains alpha.
        /// </summary>
        public const int ColorHalf = 8;
        
    }
}