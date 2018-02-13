namespace HhhPrefabManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using System;

    public class Test : MonoBehaviour
    {
        public PrefabType prefab;
        List<Transform> list = new List<Transform>();

        public float delay1 = 0.5f, delay2 = 5f;
        public Vector2 speed = new Vector2(5f, 15f);

        private void Start()
        {
            StartCoroutine("Spawn");
        }

        IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(delay1);

                var pp = PrefabManager.instance.Spawn(prefab);
                list.Add(pp.transform);
                StartCoroutine(DestroyPP(pp));
            }
        }

        IEnumerator DestroyPP(IPrefabComponent pp)
        {
            yield return new WaitForSeconds(delay2);
            list.Remove(pp.transform);
            PrefabManager.instance.Return(pp);
        }

        private void Update()
        {
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i].transform;
                t.position += t.forward * speed.x * Time.deltaTime;
                t.Rotate(0, speed.y * Time.deltaTime, 0);
            }
        }
    }
}