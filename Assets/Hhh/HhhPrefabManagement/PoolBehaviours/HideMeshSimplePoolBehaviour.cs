namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class HideMeshSimplePoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        public void OnReturnToPool()
        {
            _renderer.enabled = false;
        }

        public void OnSpawnFromPool()
        {
            _renderer.enabled = true;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _renderer = GetComponentInChildren<Renderer>();
        }
    }
}