namespace HhhPrefabManagement
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExtensionsInstantiateDestroy
    {

        /// <summary>
        /// Safely destroys the supplied GameObject. Uses <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/> when in 'Play' mode, and <see cref="UnityEngine.Object.DestroyImmediate(UnityEngine.Object)"/> otherwise.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        public static void DestroySafe(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                UnityEngine.Object.Destroy(gameObject);
            }

#if UNITY_EDITOR
            else
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
#endif
        }

        /// <summary>
        /// Safely destroys the supplied GameObject. Uses <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/> when in 'Play' mode, and <see cref="UnityEngine.Object.DestroyImmediate(UnityEngine.Object)"/> otherwise.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="gameObject">The game object.</param>
        /// <seealso cref="DestroySafe(GameObject)"/>
        public static void DestroySafe(this Component c, GameObject gameObject)
        {
            DestroySafe(gameObject);
        }

        public static GameObject InstantiateSafe(this Component c, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return InstantiateSafe(prefab, position, rotation, parent);
        }

        public static GameObject InstantiateSafe(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return (parent != null ?
                        UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation, parent) :
                        UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation));
            }
            else
            {
                var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                go.transform.position = position;
                go.transform.rotation = rotation;
                if (parent != null)
                {
                    go.transform.SetParent(parent, true);
                }

                return go;
            }

#else
            return (parent != null ?
                    UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation, parent) :
                    UnityEngine.Object.Instantiate<GameObject>(prefab, position, rotation));

#endif
        }

    }
}
