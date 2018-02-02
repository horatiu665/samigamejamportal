namespace HhhNetwork
{
    using System;
    using System.Text;
    using UnityEngine;

    public static class NetExtensions
    {
        //public static int GetByteSize(this VRPrefabType type)
        //{
        //    var typeMode = VRPrefabManager.instance.prefabTypeMode;
        //    switch (typeMode)
        //    {
        //        case VRPrefabTypeMode.Byte:
        //        {
        //            return 1;
        //        }

        //        case VRPrefabTypeMode.Short:
        //        {
        //            return 2;
        //        }

        //        case VRPrefabTypeMode.Int:
        //        {
        //            return 4;
        //        }

        //        default:
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }
        //}

        /// <summary>
        /// Converts this <see cref="string"/> to a byte array.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Gets the size of this <see cref="string"/> in bytes.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static int GetByteSize(this string str)
        {
            return Encoding.UTF8.GetByteCount(str);
        }

        /// <summary>
        /// Converts the given <see cref="byte[]"/> to a string, starting from index and ending at count.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        /// <summary>
        /// Gets a subset of this <see cref="byte[]"/>, starting from 0 and ending at count.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static byte[] Subset(this byte[] bytes, int count)
        {
            return Subset(bytes, 0, count);
        }

        /// <summary>
        /// Gets a subset of this <see cref="byte[]"/>, starting from startIdx and ending at count.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="startIdx">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static byte[] Subset(this byte[] bytes, int startIdx, int count)
        {
            var arr = new byte[count];
            Buffer.BlockCopy(bytes, startIdx, arr, 0, count);
            return arr;
        }

        /// <summary>
        /// Converts this <see cref="float"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int Wrap(this float value)
        {
            var u = new FloatIntUnion()
            {
                f = value
            };

            return u.i;
        }

        /// <summary>
        /// Converts this <see cref="int"/> to a <see cref="float"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static float Unwrap(this int value)
        {
            var u = new FloatIntUnion()
            {
                i = value
            };

            return u.f;
        }

        /// <summary>
        /// Converts the byte array to a string with 0s and 1s representing the contents of the byte array.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public static string ToStringUnconverted(this byte[] buffer)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString());
            }

            return sb.ToString();
        }

    }
}