namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class PrefabComponentBase : MonoBehaviour, IPrefabComponent
    {
        [SerializeField]
        private PrefabType _prefabType;
        public PrefabType prefabType
        {
            get
            {
                return _prefabType;
            }
        }

        public Vector3 localPosition
        {
            get { return this.transform.localPosition; }
            set { this.transform.localPosition = value; }
        }

        public Vector3 position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        public Quaternion localRotation
        {
            get { return this.transform.localRotation; }
            set { this.transform.localRotation = value; }
        }

        public Quaternion rotation
        {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; }
        }

        public void SetPrefabType(PrefabType prefabType)
        {
            _prefabType = prefabType;
        }

    }
}