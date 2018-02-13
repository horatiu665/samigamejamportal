namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class HideMeshMultiPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        [SerializeField]
        private Renderer[] _renderers;

        public void OnReturnToPool()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = false;
            }
        }

        public void OnSpawnFromPool()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = true;
            }
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _renderers = GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
        }
    }
}