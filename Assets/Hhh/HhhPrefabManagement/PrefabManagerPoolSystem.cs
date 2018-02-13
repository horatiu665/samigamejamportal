namespace HhhPrefabManagement
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class PrefabManagerPoolSystem
    {
        public static Dictionary<PrefabType, Stack<IPrefabComponent>> prefabs = new Dictionary<PrefabType, Stack<IPrefabComponent>>();

        public static void InitAllPools(int instanceCount)
        {
            var allPrefabTypes = (PrefabType[])System.Enum.GetValues(typeof(PrefabType));
            for (int i = 0; i < allPrefabTypes.Length; i++)
            {
                // set up pool for this prefab type
                InitPool(allPrefabTypes[i], instanceCount);
            }
        }

        public static void InitPool(PrefabType prefabType, int instanceCount)
        {
            ClearPool(prefabType);

            for (int i = 0; i < instanceCount; i++)
            {
                var pp = SpawnOne(prefabType, PrefabManager.instance.transform);
                Return(pp);
            }
        }

        public static void ClearPool(PrefabType prefabType)
        {
            if (prefabs.ContainsKey(prefabType))
            {
                while (prefabs[prefabType].Count > 0)
                {
                    var pp = prefabs[prefabType].Pop();
                    ExtensionsInstantiateDestroy.DestroySafe(pp.gameObject);
                }
                prefabs.Clear();
            }
            else
            {
                prefabs[prefabType] = new Stack<IPrefabComponent>();
            }
        }

        private static IPrefabComponent SpawnOne(PrefabType prefabType, Transform parent = null)
        {
            var prefab = PrefabManager.instance.GetPrefab<IPrefabComponent>(prefabType);
            if (parent == null)
            {
                return MonoBehaviour.Instantiate(prefab.gameObject).GetComponent<IPrefabComponent>();
            }
            else
            {
                return MonoBehaviour.Instantiate(prefab.gameObject, parent).GetComponent<IPrefabComponent>();
            }
        }

        public static IPrefabComponent Get(PrefabType prefabType, Vector3 pos, Quaternion rot)
        {
            var p = Get(prefabType);
            p.position = pos;
            p.rotation = rot;
            return p;
        }

        public static IPrefabComponent Get(PrefabType prefabType)
        {
            if (prefabs.ContainsKey(prefabType) && prefabs[prefabType].Count > 0)
            {
                var pp = prefabs[prefabType].Pop();
                // because interfaces can be null and not detected by unity, we will use gameObject null check
                if (pp.gameObject != null)
                {
                    ExecuteSpawnPoolBehaviours(pp);
                    return pp;
                }
                else
                {
                    Debug.LogError("[PrefabManagerPoolSystem] Null gameObject in pool!!! This should not happen!");
                    return null;
                }
            }
            else
            {
                var pp = SpawnOne(prefabType);
                ExecuteSpawnPoolBehaviours(pp);
                return pp;
            }
        }

        public static void Return(IPrefabComponent item)
        {
            if (prefabs.ContainsKey(item.prefabType))
            {
                ExecuteReturnPoolBehaviours(item);

                prefabs[item.prefabType].Push(item);
            }
        }

        private static void ExecuteSpawnPoolBehaviours(IPrefabComponent item)
        {
            // pool behaviours for spawning (ex: enable meshes, colliders, gameobject or initialize some values).
            var poolBehs = item.gameObject.GetComponents<IPoolBehaviour>();
            for (int i = 0; i < poolBehs.Length; i++)
            {
                poolBehs[i].OnSpawnFromPool();
            }
        }

        private static void ExecuteReturnPoolBehaviours(IPrefabComponent item)
        {
            // pool behaviours for returning (ex: disable meshes and colliders, disable gameobject, parent somewhere)
            var poolBehs = item.gameObject.GetComponents<IPoolBehaviour>();
            for (int i = 0; i < poolBehs.Length; i++)
            {
                poolBehs[i].OnReturnToPool();
            }
        }
    }
}
