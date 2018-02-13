namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;
    using UnityEngine;

    public class SimpleCubeSpawnerDebugComponent : MonoBehaviour
    {
        public PrefabType prefabType = 0;
        public float upForcePower = 10f;
        public float spawnFrequency = 1.5f;
        public int maxRigidbodies = 100;

        private float _lastSpawn;
        private int counter;

        private void OnEnable()
        {
            _lastSpawn = Time.timeSinceLevelLoad + this.spawnFrequency;
        }

        private void Update()
        {
            if (!NetServices.isServer)
            {
                return;
            }

            var time = Time.timeSinceLevelLoad;
            if (time < _lastSpawn)
            {
                return;
            }

            if (counter++ >= this.maxRigidbodies)
            {
                return;
            }

            _lastSpawn = time + this.spawnFrequency;
            var rb = ServerRbSyncManager.instance.HandleRigidbodySyncSpawn(this.prefabType, this.transform.position, this.transform.rotation);
            if (rb != null)
            {
                rb.GetComponent<Rigidbody>().AddForce((Random.insideUnitSphere + rb.transform.up) * this.upForcePower, ForceMode.Impulse);
            }
        }
    }
}