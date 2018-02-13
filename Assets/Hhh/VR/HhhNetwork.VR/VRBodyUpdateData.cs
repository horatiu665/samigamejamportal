namespace HhhNetwork.VR
{
    using UnityEngine;

    public struct VRBodyUpdateData
    {
        public Vector3 position;

        public Vector3 headPosition;
        public Quaternion headRotation;

        public Vector3 leftHandPosition;
        public Quaternion leftHandRotation;

        public Vector3 rightHandPosition;
        public Quaternion rightHandRotation;
    }
}