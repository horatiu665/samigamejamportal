namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class DisablePhysicsPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        [SerializeField]
        private Collider[] _cols;

        public void OnReturnToPool()
        {
            for (int i = 0; i < _cols.Length; i++)
            {
                _cols[i].enabled = false;
            }
        }

        public void OnSpawnFromPool()
        {
            for (int i = 0; i < _cols.Length; i++)
            {
                _cols[i].enabled = true;
            }
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _cols = GetComponentsInChildren<Collider>(true);
        }

    }
}