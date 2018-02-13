namespace HhhVRGrabber
{
    using HhhNetwork;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ThrowMessage : NetIdMessageBase
    {
        public ThrowMessage(byte[] buffer) : base(buffer)
        {
        }

        public ThrowMessage() : base(NetMessageType.Throw)
        {
        }

        // need the RB id
        public int syncId
        {
            get;
            set;
        }

        public bool leftHand
        {
            get;
            set;
        }

        public Vector3 position
        {
            get;
            set;
        }

        public Quaternion rotation
        {
            get;
            set;
        }

        public Vector3 velocity
        {
            get;
            set;
        }

        public Vector3 angularVelocity
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return base.GetByteSize() + 31; // equippable net id (4) + is left (1) + position (6) + rotation (8) + velocity (6) + angular velocity (6)
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(this.syncId);
            s.Write(this.leftHand);
            s.Write(this.position);
            s.Write(this.rotation);
            s.Write(this.velocity);
            s.Write(this.angularVelocity);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            this.syncId = s.ReadInt();
            this.leftHand = s.ReadBool();
            this.position = s.ReadVector3();
            this.rotation = s.ReadQuaternion();
            this.velocity = s.ReadVector3();
            this.angularVelocity = s.ReadVector3();
        }

    }
}