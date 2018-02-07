namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ScoreBigScreenPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        [SerializeField]
        private MeshRenderer[] _renderers;
        [SerializeField]
        private Canvas _canvas;

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
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = active;
                _canvas.gameObject.SetActive(active);
            }
        }

        private void OnValidate()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _canvas = GetComponentInChildren<Canvas>();
        }

    }
}