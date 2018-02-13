namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;
    using System.Collections.Generic;
    using UnityEngine;

    public class RigidbodySyncUpdateMessage : DataMessage
    {
        public RigidbodySyncUpdateMessage(byte[] buffer) : base(buffer)
        {
        }

        public RigidbodySyncUpdateMessage() : base(NetMessageType.RigidbodySyncUpdate)
        {
        }

        public static int bytesPerData
        {
            get
            {
                // sync id (4) + precise position (12) + imprecise quaternion (8) + imprecise vel and AV (3 * 2 + 3 * 2) + VR Prefab Type
                return 36 + ((PrefabType)0).GetByteSize();
            }
        }

        public List<RigidbodySyncData> data
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether this message is an initial setup message for a new player joining.
        /// </summary>
        public bool initial
        {
            get;
            set;
        }

        public void SetData(RigidbodySyncData[] data)
        {
            if (this.data == null)
            {
                this.data = new List<RigidbodySyncData>(data.Length);
            }
            else
            {
                this.data.Clear();
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].IsNull())
                {
                    // the first "null" (default valued) element we encounter means the end of the actual data
                    break;
                }

                this.data.Add(data[i]);
            }
        }

        protected override int GetByteSize()
        {
            return 2 + (data.Count * bytesPerData); // initial (1) + count (1) + data
        }

        protected override void Serialize(NetSerializer s)
        {
            s.Write(this.initial);

            var data = this.data;
            var count = data.Count;
            s.Write((byte)count);

            for (int i = 0; i < count; i++)
            {
                var d = data[i];
                s.Write(d.syncId);
                s.Write(d.prefabType);
                s.WriteExact(d.position);
                s.Write(d.rotation);
                s.Write(d.velocity);
                s.Write(d.angularVelocity);
            }
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.initial = s.ReadBool();

            var count = s.ReadByte();
            if (this.data == null)
            {
                this.data = new List<RigidbodySyncData>(count);
            }
            else
            {
                this.data.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                this.data.Add(new RigidbodySyncData()
                {
                    syncId = s.ReadInt(),
                    prefabType = s.ReadPrefabType(),
                    position = s.ReadExactVector3(),
                    rotation = s.ReadQuaternion(),
                    velocity = s.ReadVector3(),
                    angularVelocity = s.ReadVector3()
                });
            }
        }
    }
}