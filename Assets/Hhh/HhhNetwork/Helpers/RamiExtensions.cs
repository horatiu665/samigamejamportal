namespace HhhNetwork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class RamiExtensions
    {
        //private static int _nextPoolId = 0;

        ///// <summary>
        ///// Sets the pool identifier.
        ///// </summary>
        ///// <param name="pooled">The pooled.</param>
        //public static void SetPoolId(this IPooledIdentity pooled)
        //{
        //    if (_nextPoolId == int.MaxValue)
        //    {
        //        _nextPoolId = 0;
        //    }

        //    pooled.ReId(_nextPoolId++);
        //}

        /// <summary>
        /// Destroys all child game objects.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public static void ClearChildren(this Transform transform)
        {
            var count = transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                transform.DestroySafe(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Updates the configurable joint's connected anchor setting
        /// </summary>
        /// <param name="joint">The joint.</param>
        public static void UpdateConnectedAnchor(this ConfigurableJoint joint)
        {
            joint.connectedAnchor = joint.connectedAnchor;
        }

        /// <summary>
        /// Updates all the configurable joints' connected anchor setting
        /// </summary>
        /// <param name="joints">The joints.</param>
        public static void UpdateConnectedAnchors(this IList<ConfigurableJoint> joints)
        {
            var count = joints.Count;
            for (int i = 0; i < count; i++)
            {
                joints[i].UpdateConnectedAnchor();
            }
        }

        ///// <summary>
        ///// Gets the <see cref="IVRCustomBehaviour"/>s for this <see cref="IVRSpawnable"/>.
        ///// If the spawnable has <see cref="IVRSpawnable.mixWithPlayerBehaviours"/> set to true, behaviours from both the spawnable and the <see cref="IVRPlayer"/> are combined into one list.
        ///// Otherwise, only the spawnable's behaviours are returned if any exist, or if none exist on the spawnable, the player's behaviours are returned.
        ///// </summary>
        ///// <param name="spawnable">The spawnable.</param>
        ///// <param name="player">The player.</param>
        ///// <returns></returns>
        //public static IList<IVRCustomBehaviour> GetBehaviours(this IVRSpawnable spawnable, IVRPlayer player)
        //{
        //    IList<IVRCustomBehaviour> behaviours = null;
        //    if (!spawnable.mixWithPlayerBehaviours)
        //    {
        //        behaviours = spawnable.behaviours;
        //        if (behaviours == null || behaviours.Count == 0)
        //        {
        //            behaviours = player.behaviours;
        //        }
        //    }
        //    else
        //    {
        //        var spawnableBehaviours = spawnable.behaviours;
        //        var playerBehaviours = player.behaviours;

        //        var b = new List<IVRCustomBehaviour>(spawnableBehaviours.Count + playerBehaviours.Count);
        //        b.AddRange(spawnableBehaviours);
        //        b.AddRange(playerBehaviours);

        //        behaviours = b;
        //    }

        //    return behaviours;
        //}

        ///// <summary>
        ///// Gets the <see cref="IVRCustomBehaviour"/>s for this <see cref="IVREquippable"/>.
        ///// If the equippable has <see cref="IVREquippable.mixWithPlayerBehaviours"/> set to true, behaviours from both the equippable and the <see cref="IVRPlayer"/> are combined into one list.
        ///// Otherwise, only the equippable's behaviours are returned if any exist, or if none exist on the equippable, the player's behaviours are returned.
        ///// </summary>
        ///// <param name="equippable">The equippable.</param>
        ///// <param name="player">The player.</param>
        ///// <returns></returns>
        //public static IList<IVRCustomBehaviour> GetBehaviours(this IVREquippable equippable, IVRPlayer player)
        //{
        //    IList<IVRCustomBehaviour> behaviours = null;
        //    if (!equippable.mixWithPlayerBehaviours)
        //    {
        //        behaviours = equippable.behaviours;
        //        if (behaviours == null || behaviours.Count == 0)
        //        {
        //            behaviours = player.behaviours;
        //        }
        //    }
        //    else
        //    {
        //        var equippableBehaviours = equippable.behaviours;
        //        var playerBehaviours = player.behaviours;

        //        var b = new List<IVRCustomBehaviour>(equippableBehaviours.Count + playerBehaviours.Count);
        //        b.AddRange(equippableBehaviours);
        //        b.AddRange(playerBehaviours);

        //        behaviours = b;
        //    }

        //    return behaviours;
        //}

        /// <summary>
        /// Returns the last element in the list, or <see cref="default{T}"/> if list is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static T Last<T>(this IList<T> list)
        {
            return list.Count > 0 ? list[list.Count - 1] : default(T);
        }

        /// <summary>
        /// Returns a random element in the list, or <see cref="default{T}"/> if list is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static T Random<T>(this IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
            {
                return default(T);
            }

            return count > 1 ? list[UnityEngine.Random.Range(0, count)] : list[0];
        }

        // this is the bad C# sort. please never use this
        ///// <summary>
        ///// Sorts this list using the supplied comparison. This is done by casting to <see cref="List{T}"/>.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list">The list.</param>
        ///// <param name="comparison">The comparison.</param>
        //public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        //{
        //    ((List<T>)list).Sort(comparison);
        //}

        /// <summary>
        /// Adds the supplied enumerable to this list. This is done by casting to <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="enumerable">The enumerable.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> enumerable)
        {
            ((List<T>)list).AddRange(enumerable);
        }

        /// <summary>
        /// Clears the entire specified array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        /// Determines whether this Quaternion is zero.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <returns>
        ///   <c>true</c> if the specified quaternion is zero; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsZero(this Quaternion quaternion)
        {
            return quaternion.x == 0f &&
                   quaternion.y == 0f &&
                   quaternion.z == 0f &&
                   quaternion.w == 0f;
        }

        /// <summary>
        /// Returns the value in this dictionary matching the supplied key, or the default value if no match exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue val;
            if (dict.TryGetValue(key, out val))
            {
                return val;
            }

            return defaultValue;
        }

        //public static T GetComponent<T>(this IVRPrefab prefab) where T : Component
        //{
        //    return prefab.gameObject.GetComponent<T>();
        //}

        //public static T AddComponent<T>(this IVRPrefab prefab) where T : Component
        //{
        //    return prefab.gameObject.AddComponent<T>();
        //}

        //public static T GetOrAddComponent<T>(this IVRPrefab prefab) where T : Component
        //{
        //    return prefab.gameObject.GetOrAddComponent<T>();
        //}

        public static T AddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            var comp = c.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            return c.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            return go.AddComponent<T>();
        }

        public static Color ChangeAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static void ColorRenderers(this Component c, Color color, bool includeChildren = true)
        {
            var renderers = GetAllRenderers(c, includeChildren);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = color;
            }
        }

        public static Renderer[] GetAllRenderers(this Component c, bool includeChildren = true)
        {
            return includeChildren ? c.GetComponentsInChildren<Renderer>() : c.GetComponents<Renderer>();
        }

        public static T[] Concat<T>(this T[] x, T[] y)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            var oldLen = x.Length;
            Array.Resize(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);

            return x;
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            var dest = new T[source.Length - 1];
            if (index > 0)
            {
                Array.Copy(source, 0, dest, 0, index);
            }

            if (index < source.Length - 1)
            {
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
            }

            return dest;
        }

        public static int IndexOf<T>(this T[] array, T element)
        {
            return Array.IndexOf(array, element);
        }

        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            var subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }

            return subset;
        }

        public static T[] Subset<T>(this T[] array, int startIndex, int length)
        {
            var subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        /// <summary>
        /// Returns a random float between the x [inclusive] and y [inclusive] of the given <see cref="Vector2"/>
        /// </summary>
        /// <param name="vec">The Vector2.</param>
        /// <returns></returns>
        public static float Random(this Vector2 vec)
        {
            return UnityEngine.Random.Range(vec.x, vec.y);
        }

        /// <summary>
        /// Returns a random integer between x [inclusive] and y [exclusive] of the given <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vec">The Vector2.</param>
        /// <returns></returns>
        public static int RandomInt(this Vector2 vec)
        {
            return UnityEngine.Random.Range(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }

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

        /// <summary>
        /// Sets a range of 'magic' values on the given <see cref="ConfigurableJoint"/> in order to setup the joint as a 'hand joint' for VR use.
        /// </summary>
        /// <param name="joint">The joint.</param>
        public static void SetDefaultHandJointSettings(this ConfigurableJoint joint)
        {
            joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Free;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive()
            {
                positionSpring = 200000,
                positionDamper = 1,
                maximumForce = float.MaxValue
            };
        }
    }
}