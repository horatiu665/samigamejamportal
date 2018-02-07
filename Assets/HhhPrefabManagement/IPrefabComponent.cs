namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public interface IPrefabComponent
    {
        /// <summary>
        /// Generated prefab type. <see cref="PrefabManager"/>
        /// </summary>
        PrefabType prefabType { get; }

        // monobehaviour

        Transform transform { get; }

        GameObject gameObject { get; }
        
        // quick access to transform. for ease of finding references
        Vector3 position { get; set; }
        
        Vector3 localPosition { get; set; }
        
        Quaternion rotation { get; set; }
        
        Quaternion localRotation { get; set; }

        /// <summary>
        /// Sets prefab type. Separate for ease of finding references and debug
        /// </summary>
        void SetPrefabType(PrefabType prefabType);

    }
}