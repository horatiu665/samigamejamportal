namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class PrefabManager : MonoBehaviour
    {
        private static PrefabManager _instance;
        public static PrefabManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PrefabManager>();
                }
                if (_instance == null)
                {
                    Debug.LogError("[PrefabManager] Instance not found. Must set up in scene (find manager prefab) - because of the code generation aspects and prefab references and such.");
                }
                return _instance;
            }
        }

        public static event System.Action<IPrefabComponent> OnSpawn;

        [Space]
        [SerializeField, Tooltip("Decides what the constraining type is for the auto-generated PrefabType enum. Basically controls how many different prefabs there may be registered.")]
        protected PrefabTypeMode _prefabTypeMode = PrefabTypeMode.Byte;

        /// <summary>
        /// Gets the currently selected prefab type mode, determining the base type of the auto-generated PrefabType enum.
        /// </summary>
        /// <value>
        /// The prefab type mode.
        /// </value>
        public PrefabTypeMode prefabTypeMode
        {
            get { return _prefabTypeMode; }
        }

        [SerializeField, Tooltip("The initial preallocation count for prefab pools.")]
        protected int _initialInstanceCount = 10;

        [Header("Just for testing a PrefabType field")]
        public PrefabType favoritePrefab;

        [Header("Generates PrefabType enum")]
        [SerializeField, Tooltip("Drag all prefabs into this field. The PrefabType enum is auto-generated based on this array of prefabs.")]
        protected GameObject[] _prefabs = new GameObject[0];

#if UNITY_EDITOR

        /// <summary>
        /// Called by Unity when an inspector change occurs or when scripts recompile - only called in the editor.
        /// </summary>
        protected virtual void OnValidate()
        {
            SetPrefabTypes();
        }

#endif

        /// <summary>
        /// Sets prefab type on all the assigned prefabs, which allows them to be scattered in the project structure
        /// </summary>
        private void SetPrefabTypes()
        {
            for (int i = 0; i < _prefabs.Length; i++)
            {
                if (_prefabs[i] != null)
                {
                    _prefabs[i].GetComponent<IPrefabComponent>().SetPrefabType((PrefabType)i);
                }
            }
        }

        private void Awake()
        {
            PrefabManagerPoolSystem.InitAllPools(_initialInstanceCount);
        }

        /// <summary>
        /// Gets the prefab component associated with the given PrefabType
        /// </summary>
        public virtual TPrefab GetPrefab<TPrefab>(PrefabType prefabType) where TPrefab : class, IPrefabComponent
        {
            for (int i = 0; i < _prefabs.Length; i++)
            {
                var prefab = _prefabs[i];
                if (_prefabs[i] == null)
                {
                    continue;
                }
                var comp = prefab.GetComponent<TPrefab>();
                if (comp == null)
                {
                    continue;
                }

                if (comp.prefabType != prefabType)
                {
                    continue;
                }

                return comp;
            }

            return null;
        }

        public virtual TPrefab Spawn<TPrefab>(PrefabType type) where TPrefab : class, IPrefabComponent
        {
            return Spawn<TPrefab>(type, Vector3.zero, Quaternion.identity);
        }

        public virtual TPrefab Spawn<TPrefab>(PrefabType type, Vector3 position, Quaternion rotation) where TPrefab : class, IPrefabComponent
        {
            var prefab = Spawn(type, position, rotation);
            if (prefab != null)
            {
                var cast = prefab as TPrefab;
                if (cast != null)
                {
                    return cast;
                }

                return prefab.transform.GetComponent<TPrefab>();
            }

            return null;
        }

        public virtual IPrefabComponent Spawn(PrefabType type)
        {
            return Spawn(type, Vector3.zero, Quaternion.identity);
        }

        public virtual IPrefabComponent Spawn(PrefabType type, Vector3 position, Quaternion rotation)
        {
            IPrefabComponent prefab;

#if UNITY_EDITOR
            // if not playing, use the prefab mode
            if (!Application.isPlaying)
            {
                var prefabgo = this.InstantiateSafe(_prefabs[(int)type], position, rotation);
                prefab = prefabgo.GetComponent<IPrefabComponent>();
            }
            else
            {
#endif
                // if playing, use the pools
                prefab = PrefabManagerPoolSystem.Get(type, position, rotation);

#if UNITY_EDITOR
            }
#endif
            prefab.SetPrefabType(type);

            if (OnSpawn != null)
            {
                OnSpawn(prefab);
            }

            return prefab;
        }

        public virtual void Return(IPrefabComponent prefab)
        {

#if UNITY_EDITOR
            // if not playing, use the prefab mode
            if (!Application.isPlaying)
            {
                this.DestroySafe(prefab.gameObject);
            }
            else
            {
#endif
                // if playing, use the pools
                PrefabManagerPoolSystem.Return(prefab);

#if UNITY_EDITOR
            }
#endif
        }

    }
}