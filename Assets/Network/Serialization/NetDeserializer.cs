namespace HhhNetwork
{
    using System;
    using UnityEngine;

    public class NetDeserializer
    {
        private byte[] _buffer;
        private int _currentIndex;

        public NetDeserializer()
        {
        }

        internal void SetBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _currentIndex = 0;
        }

        /* Read methods */

        /// <summary>
        /// Reads a <see cref="NetMessageType"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public NetMessageType ReadType()
        {
            return (NetMessageType)ReadByte();
        }

        /// <summary>
        /// Reads the next <see cref="VRPrefabType"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        //public VRPrefabType ReadPrefabType()
        //{
        //    var typeMode = VRPrefabManager.instance.prefabTypeMode;
        //    switch (typeMode)
        //    {
        //        case VRPrefabTypeMode.Byte:
        //        {
        //            return (VRPrefabType)ReadByte();
        //        }

        //        case VRPrefabTypeMode.Short:
        //        {
        //            return (VRPrefabType)ReadShort();
        //        }

        //        case VRPrefabTypeMode.Int:
        //        {
        //            return (VRPrefabType)ReadInt();
        //        }

        //        default:
        //        {
        //            throw new System.NotImplementedException();
        //        }
        //    }
        //}

        /// <summary>
        /// Reads a <see cref="bool"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            return ReadByte() > 0;
        }

        /// <summary>
        /// Reads a signed <see cref="short"/> (half) from the buffer.
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            var result = BitConverter.ToInt16(_buffer, _currentIndex);
            _currentIndex += sizeof(short);
            return result;
        }

        /// <summary>
        /// Reads an unsigned <see cref="ushort"/> (half) from the buffer.
        /// </summary>
        /// <returns></returns>
        public ushort ReadHalf()
        {
            var result = BitConverter.ToUInt16(_buffer, _currentIndex);
            _currentIndex += sizeof(ushort);
            return result;
        }

        /// <summary>
        /// Reads an <see cref="int"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            var result = BitConverter.ToInt32(_buffer, _currentIndex);
            _currentIndex += sizeof(int);
            return result;
        }

        /// <summary>
        /// Reads a <see cref="ulong"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public ulong ReadULong()
        {
            var result = BitConverter.ToUInt64(_buffer, _currentIndex);
            _currentIndex += sizeof(ulong);
            return result;
        }

        /// <summary>
        /// Reads a <see cref="float"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            return ReadInt().Unwrap();
        }

        /// <summary>
        /// Reads a <see cref="string"/> from the buffer. Please note that this method expects that before the string, a single byte tells the length of the string.
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            // TODO: find out what is up with string read / writing
            var length = ReadByte(); // the first byte must tell the string length
            if (length == 0)
            {
                return string.Empty;
            }

            var result = _buffer.GetString(_currentIndex, length);
            _currentIndex += length;
            return result.TrimEnd('\0');
        }

        /// <summary>
        /// Reads a <see cref="Vector2"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public Vector2 ReadVector2()
        {
            var x = Mathf.HalfToFloat(ReadHalf());
            var y = Mathf.HalfToFloat(ReadHalf());
            return new Vector2(x, y);
        }

        /// <summary>
        /// Reads a <see cref="Vector3"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public Vector3 ReadVector3()
        {
            var x = Mathf.HalfToFloat(ReadHalf());
            var y = Mathf.HalfToFloat(ReadHalf());
            var z = Mathf.HalfToFloat(ReadHalf());
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Reads an exact <see cref="Vector2"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public Vector3 ReadExactVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        /// <summary>
        /// Reads an exact <see cref="Vector3"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public Vector3 ReadExactVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        /// <summary>
        /// Reads a <see cref="Color"/> from the buffer.
        /// </summary>
        /// <param name="readAlpha">if set to <c>true</c>, expects to read an alpha channel (one byte more).</param>
        /// <returns></returns>
        public Color ReadColor(bool readAlpha = true)
        {
            var r = Mathf.HalfToFloat(ReadHalf());
            var g = Mathf.HalfToFloat(ReadHalf());
            var b = Mathf.HalfToFloat(ReadHalf());
            var a = readAlpha ? Mathf.HalfToFloat(ReadHalf()) : 1f;
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Reads a <see cref="Quaternion"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public Quaternion ReadQuaternion()
        {
            var x = Mathf.HalfToFloat(ReadHalf());
            var y = Mathf.HalfToFloat(ReadHalf());
            var z = Mathf.HalfToFloat(ReadHalf());
            var w = Mathf.HalfToFloat(ReadHalf());
            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// Reads the next <see cref="byte"/> from the buffer.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return _buffer[_currentIndex++];
        }

        /// <summary>
        /// Reads the next bytes from the buffer.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            var result = _buffer.Subset(_currentIndex, count);
            _currentIndex += count;
            return result;
        }
    }
}