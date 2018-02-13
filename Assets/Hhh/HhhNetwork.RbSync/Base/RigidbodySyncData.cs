namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;
    using UnityEngine;

    public struct RigidbodySyncData
    {
        public int syncId;
        public PrefabType prefabType;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public bool IsNull()
        {
            return syncId == 0 &&
                   prefabType == 0 &&
                   position == Vector3.zero &&
                   rotation.IsZero() &&
                   velocity == Vector3.zero &&
                   angularVelocity == Vector3.zero;
        }

        public override string ToString()
        {
            return string.Concat("RigidbodySyncData:: Sync ID == " + syncId
                + ", prefab type == " + prefabType.ToString()
                + ", position == " + position.ToString()
                + ", rotation == " + rotation.ToString()
                + ", velocity == " + velocity.ToString()
                + ", aVelocity == " + angularVelocity.ToString()
                );
        }
    }
}