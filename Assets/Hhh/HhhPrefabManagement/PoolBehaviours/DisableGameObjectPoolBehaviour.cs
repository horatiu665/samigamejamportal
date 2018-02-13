namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class DisableGameObjectPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        public void OnReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public void OnSpawnFromPool()
        {
            gameObject.SetActive(true);
        }

    }
}