namespace HhhNetwork
{
    using System;
    using UnityEngine;

    public class NetSerializer
    {
        public static readonly NetSerializer instance = new NetSerializer();

        private byte[] _buffer;
        private int _currentIndex;

        internal byte[] buffer
        {
            get { return _buffer; }
        }

        internal void EnsureBufferSize(int byteSize)
        {
            if (_buffer == null || _buffer.Length < byteSize)
            {
                _buffer = new byte[byteSize];
            }
        }

        internal void ResetIndex()
        {
            _currentIndex = 0;
        }

        /// <summary>
        /// Writes the specified <see cref="NetMessageType"/> to the buffer.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Write(NetMessageType type)
        {
            Write((byte)type);
        }

        ///// <summary>
        ///// Writes the specified <see cref="VRPrefabType"/>.
        ///// </summary>
        ///// <param name="type">The type.</param>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public void Write(VRPrefabType type)
        //{
        //    var typeMode = VRPrefabManager.instance.prefabTypeMode;
        //    switch (typeMode)
        //    {
        //        case VRPrefabTypeMode.Byte:
        //        {
        //            Write((byte)type);
        //            break;
        //        }

        //        case VRPrefabTypeMode.Short:
        //        {
        //            Write((short)type);
        //            break;
        //        }

        //        case VRPrefabTypeMode.Int:
        //        {
        //            Write((int)type);
        //            break;
        //        }

        //        default:
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //}

        /// <summary>
        /// Writes the specified <see cref="bool"/> to the buffer.
        /// </summary>
        /// <param name="value"></param>
        public void Write(bool value)
        {
            Write((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Writes the specified <see cref="short"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(short value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
        }

        /// <summary>
        /// Writes the specified <see cref="ushort"/> (half) to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(ushort value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
        }

        /// <summary>
        /// Writes the specified <see cref="int"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(int value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
            Write((byte)(value >> 16));
            Write((byte)(value >> 24));
        }

        /// <summary>
        /// Writes the specified <see cref="long"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(long value)
        {
            Write((ulong)value);
        }

        /// <summary>
        /// Writes the specified <see cref="ulong"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(ulong value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
            Write((byte)(value >> 16));
            Write((byte)(value >> 24));
            Write((byte)(value >> 32));
            Write((byte)(value >> 40));
            Write((byte)(value >> 48));
            Write((byte)(value >> 56));
        }

        /// <summary>
        /// Writes the specified <see cref="float"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(float value)
        {
            Write(value.Wrap());
        }

        /// <summary>
        /// Writes the specified <see cref="byte"/> array to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte[] value)
        {
            Write(value, value.Length);
        }

        /// <summary>
        /// Writes the specified <see cref="byte"/> array to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte[] value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Write(value[i]);
            }
        }

        /// <summary>
        /// Writes the specified <see cref="string"/> to the buffer. Please note that before the string itself, its count is written as one byte.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(string value)
        {
            var size = (byte)Math.Min(byte.MaxValue, value.GetByteSize());
            Write(size); // the first byte must tell the string length

            if (size > 0)
            {
                var bytes = value.GetBytes();
                if (bytes.Length > size)
                {
                    bytes = bytes.Subset(size);
                }

                for (int i = 0; i < bytes.Length; i++)
                {
                    Write(bytes[i]);
                }
            }
        }

        /// <summary>
        /// Writes the specified <see cref="Vector2"/> to the buffer. BEWARE: uses half precision. See also <seealso cref="WriteExact(Vector2)"/> for high precision
        /// </summary>
        /// <param name="position">The position.</param>
        public void Write(Vector2 position)
        {
            Write(Mathf.FloatToHalf(position.x));
            Write(Mathf.FloatToHalf(position.y));
        }

        /// <summary>
        /// Writes the specified <see cref="Vector3"/> to the buffer. BEWARE: uses half precision. See also <seealso cref="WriteExact(Vector3)"/> for high precision
        /// </summary>
        /// <param name="position">The position.</param>
        public void Write(Vector3 position)
        {
            Write(Mathf.FloatToHalf(position.x));
            Write(Mathf.FloatToHalf(position.y));
            Write(Mathf.FloatToHalf(position.z));
        }

        /// <summary>
        /// Writes the exact <see cref="Vector2"/> to the buffer.
        /// </summary>
        /// <param name="position">The position.</param>
        public void WriteExact(Vector2 position)
        {
            Write(position.x);
            Write(position.y);
        }

        /// <summary>
        /// Writes the exact <see cref="Vector3"/> to the buffer.
        /// </summary>
        /// <param name="position">The position.</param>
        public void WriteExact(Vector3 position)
        {
            Write(position.x);
            Write(position.y);
            Write(position.z);
        }

        /// <summary>
        /// Writes the specified <see cref="Color"/> to the buffer, optionally including the alpha channel (one byte).
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="writeAlpha">if set to <c>true</c>, writes the alpha channel (one byte) as well.</param>
        public void Write(Color color, bool writeAlpha = true)
        {
            Write(Mathf.FloatToHalf(color.r));
            Write(Mathf.FloatToHalf(color.g));
            Write(Mathf.FloatToHalf(color.b));

            if (writeAlpha)
            {
                Write(Mathf.FloatToHalf(color.a));
            }
        }

        /// <summary>
        /// Writes the specified <see cref="Quaternion"/> to the buffer.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        public void Write(Quaternion rotation)
        {
            Write(Mathf.FloatToHalf(rotation.x));
            Write(Mathf.FloatToHalf(rotation.y));
            Write(Mathf.FloatToHalf(rotation.z));
            Write(Mathf.FloatToHalf(rotation.w));
        }

        /// <summary>
        /// Writes the specified <see cref="byte"/> to the buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte value)
        {
            _buffer[_currentIndex++] = value;
        }
    }
}