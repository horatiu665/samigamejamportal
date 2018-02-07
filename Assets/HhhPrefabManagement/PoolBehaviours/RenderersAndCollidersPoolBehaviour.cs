namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class RenderersAndCollidersPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        [SerializeField]
        private List<Renderer> _renderers;

        [SerializeField]
        private List<Collider> _colliders;

        public void OnReturnToPool()
        {
            SetActive(false);
        }

        public void OnSpawnFromPool()
        {
            SetActive(true);
        }

        private void SetActive(bool active)
        {
            if (_renderers.Any(r => r == null))
            {
                _renderers.RemoveAll(r => r == null);
            }
            if (_colliders.Any(c => c == null))
            {
                _colliders.RemoveAll(c => c == null);
            }

            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].enabled = active;
            }
            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].enabled = active;
            }
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _renderers = GetComponentsInChildren<Renderer>(true).ToList();
            _colliders = GetComponentsInChildren<Collider>(true).ToList();
        }

        public void AddRenderer(Renderer r)
        {
            this._renderers.Add(r);
        }

        public void AddCollider(Collider c)
        {
            this._colliders.Add(c);
        }

        public void RemoveRenderer(Renderer r)
        {
            _renderers.Remove(r);
        }

        public void RemoveCollider(Collider c)
        {
            _colliders.Remove(c);
        }
    }
}