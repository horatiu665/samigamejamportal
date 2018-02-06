namespace HhhNetwork
{
    using UnityEngine;

    public class VRBodyUpdateC2SMessage : DataMessage
    {
        public VRBodyUpdateC2SMessage()
            : this(NetMessageType.VRBodyUpdateC2S)
        {
        }

        public VRBodyUpdateC2SMessage(NetMessageType messageType)
            : base(messageType)
        {
        }

        public VRBodyUpdateC2SMessage(byte[] buffer)
            : base(buffer)
        {
        }

        public Vector3 position
        {
            get;
            set;
        }

        public Vector3 headPosition
        {
            get;
            set;
        }

        public Quaternion headRotation
        {
            get;
            set;
        }

        public Vector3 leftHandPosition
        {
            get;
            set;
        }

        public Quaternion leftHandRotation
        {
            get;
            set;
        }

        public Vector3 rightHandPosition
        {
            get;
            set;
        }

        public Quaternion rightHandRotation
        {
            get;
            set;
        }

        protected override int GetByteSize()
        {
            return 54; // position (12) + head position (6), head rotation (8) + lhand pos (6), lhand rot (8) + rhand pos (6), rhand rot (8)
        }

        protected override void Serialize(NetSerializer s)
        {
            s.WriteExact(this.position);

            s.Write(this.headPosition);
            s.Write(this.headRotation);

            s.Write(this.leftHandPosition);
            s.Write(this.leftHandRotation);

            s.Write(this.rightHandPosition);
            s.Write(this.rightHandRotation);
        }

        protected override void Deserialize(NetDeserializer s)
        {
            this.position = s.ReadExactVector3();

            this.headPosition = s.ReadVector3();
            this.headRotation = s.ReadQuaternion();

            this.leftHandPosition = s.ReadVector3();
            this.leftHandRotation = s.ReadQuaternion();

            this.rightHandPosition = s.ReadVector3();
            this.rightHandRotation = s.ReadQuaternion();
        }
    }
}