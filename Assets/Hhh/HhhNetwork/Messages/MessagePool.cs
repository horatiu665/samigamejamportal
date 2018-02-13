namespace HhhNetwork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A message pool for easily pooling any <see cref="DataMessage"/>-derived network messages.
    /// </summary>
    public static class MessagePool
    {
        // data structure containing pools of DataMessage-derived instances, ready for reuse. Split by type
        private static Dictionary<Type, Stack<DataMessage>> _messagePools = new Dictionary<Type, Stack<DataMessage>>();

        /// <summary>
        /// Gets a pooled <see cref="DataMessage"/>, deserialized from the given buffer.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DataMessage"/> to get.</typeparam>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <returns>A DataMessage of the specified type, serialized from the given buffer.</returns>
        public static T Get<T>(byte[] buffer) where T : DataMessage, new()
        {
            var message = Get<T>();
            message.Deserialize(buffer);
            return message;
        }

        /// <summary>
        /// Gets a pooled <see cref="NetIdMessageBase"/>, with the given net id already set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="netId">The net identifier.</param>
        /// <returns>A PlayerMessageBase of the specified type, with the netId already set.</returns>
        public static T Get<T>(short netId) where T : NetIdMessageBase, new()
        {
            var message = Get<T>();
            message.netId = netId;
            return message;
        }

        /// <summary>
        /// Gets a pooled <see cref="DataMessage"/>, in a clean state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A DataMessage of the specified type, in a clean state.</returns>
        public static T Get<T>() where T : DataMessage, new()
        {
            return GetAndCreatePoolIfMissing<T>();
        }

        private static T GetAndCreatePoolIfMissing<T>() where T : DataMessage, new()
        {
            if (!_messagePools.ContainsKey(typeof(T)))
            {
                _messagePools[typeof(T)] = new Stack<DataMessage>();
            }

            var pool = _messagePools[typeof(T)];
            if (pool != null && pool.Count > 0)
            {
                return (T)pool.Pop();
            }
            else
            {
                // this should never happen but better safe than sorry
                if (pool == null)
                {
                    _messagePools[typeof(T)] = new Stack<DataMessage>();
                }
                return new T();
            }

        }

        /// <summary>
        /// Returns the specified <see cref="DataMessage"/> to the message pool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        public static void Return<T>(T message) where T : DataMessage
        {
            if (!_messagePools.ContainsKey(typeof(T)))
            {
                _messagePools[typeof(T)] = new Stack<DataMessage>();
            }
            _messagePools[typeof(T)].Push(message);
        }
    }
}