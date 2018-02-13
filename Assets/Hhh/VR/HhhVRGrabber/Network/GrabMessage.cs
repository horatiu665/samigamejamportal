namespace HhhVRGrabber
{
    using HhhNetwork;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class GrabMessage : NetIdMessageBase
    {
        public GrabMessage(byte[] buffer) : base(buffer)
        {
        }

        public GrabMessage() : base(NetMessageType.Grab)
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

        protected override int GetByteSize()
        {
            // sync id (4) + left hand (1)
            return base.GetByteSize() + 4 + 1;
        }

        protected override void Serialize(NetSerializer s)
        {
            base.Serialize(s);
            s.Write(syncId);
            s.Write(leftHand);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            base.Deserialize(s);
            syncId = s.ReadInt();
            leftHand = s.ReadBool();
        }
    }
}